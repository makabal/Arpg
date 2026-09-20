using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerSkillController : IDisposable
{
    private readonly PlayerManager _owner;
    private readonly EnemyTargetSelector _targetSelector;
    private readonly List<PassiveSkillRuntime> _passiveSkills =
        new List<PassiveSkillRuntime>();

    private SkillRuntime _activeSkill;
    private SkillCastContext _activeContext;
    private int _activeSlot = -1;
    private bool _activeSkillReleased;
    private float _channelManaAccumulator;
    private ISkillDeliveryHandle _activeDeliveryHandle;

    public SkillDefinition ActiveDefinition => _activeSkill?.Definition;
    public SkillCastContext ActiveContext => _activeContext;
    public bool HasActiveSkill => _activeSkill != null;
    public bool ActiveSkillReleased => _activeSkillReleased;
    public int ActiveSlot => _activeSlot;
    public PlayerSkillCollection SkillCollection { get; }

    public event Action<int, SkillRuntime> SkillStarted;
    public event Action<int, SkillRuntime> SkillReleased;
    public event Action<int, SkillRuntime> SkillFinished;
    public event Action<int, SkillUseFailure> SkillFailed;

    public PlayerSkillController(
        PlayerManager owner,
        IReadOnlyList<SkillDefinition> definitions,
        EnemyTargetSelector targetSelector)
    {
        _owner = owner;
        _targetSelector = targetSelector;
        SkillCollection = new PlayerSkillCollection();

        for (int i = 0; i < PlayerManager.SkillBarSlotCount; i++)
        {
            SkillDefinition definition = definitions != null &&
                i < definitions.Count
                ? definitions[i]
                : null;

            if (definition == null)
                continue;

            PlayerSkillEntry entry =
                SkillCollection.Register(definition);

            if (definition.ActivationType != SkillActivationType.Passive ||
                HasPassiveRuntime(entry.Runtime))
            {
                continue;
            }

            var passive = new PassiveSkillRuntime(owner, entry.Runtime);
            passive.Enable();
            _passiveSkills.Add(passive);
        }
    }

    public PlayerSkillEntry GetSkillEntry(int slot)
    {
        return IsValidSlot(slot)
            ? SkillCollection.Get(_owner.GetSkillAt((SkillSlot)slot))
            : null;
    }

    public SkillRuntime GetSkill(int slot)
    {
        return GetSkillEntry(slot)?.Runtime;
    }

    public void Tick(float deltaTime)
    {
        foreach (PlayerSkillEntry entry in SkillCollection.Entries)
            entry.Runtime.Tick(deltaTime);
    }

    public SkillUseFailure TryBeginUse(int slot)
    {
        if (!IsValidSlot(slot))
            return Fail(slot, SkillUseFailure.InvalidSlot);

        PlayerSkillEntry entry = GetSkillEntry(slot);

        if (entry == null || entry.Definition == null)
            return Fail(slot, SkillUseFailure.NotEquipped);

        if (!entry.IsUnlocked)
            return Fail(slot, SkillUseFailure.SkillLocked);

        SkillRuntime runtime = entry.Runtime;
        SkillDefinition definition = entry.Definition;

        if (definition.ActivationType != SkillActivationType.Active)
            return Fail(slot, SkillUseFailure.PassiveSkill);

        if (_owner.Health.IsDead)
            return Fail(slot, SkillUseFailure.PlayerDead);

        if (_activeSkill != null)
            return Fail(slot, SkillUseFailure.Busy);

        if (!runtime.IsReady)
            return Fail(slot, SkillUseFailure.OnCooldown);

        if (_owner.Mana.CurrentValue < definition.ManaCost)
            return Fail(slot, SkillUseFailure.NotEnoughMana);

        if (definition.AimResolver == null)
            return Fail(slot, SkillUseFailure.NoAimResolver);

        if (definition.Delivery == null)
            return Fail(slot, SkillUseFailure.NoDelivery);

        Vector2 aimPosition = _targetSelector != null
            ? _targetSelector.PointerWorldPosition
            : (Vector2)_owner.transform.position;

        var aimRequest = new SkillAimRequest(
            _owner,
            _targetSelector != null
                ? _targetSelector.SelectedEnemy
                : null,
            aimPosition,
            definition.CastRange);

        SkillUseFailure aimResult = definition.AimResolver.Resolve(
            aimRequest,
            out SkillAimData aim);

        if (aimResult != SkillUseFailure.None)
            return Fail(slot, aimResult);

        var context = new SkillCastContext(
            _owner,
            definition,
            aim);

        foreach (SkillCondition condition in definition.Conditions)
        {
            if (condition == null)
                continue;

            SkillUseFailure conditionResult =
                condition.Validate(context);

            if (conditionResult != SkillUseFailure.None)
                return Fail(slot, conditionResult);
        }

        if (!_owner.Mana.TrySpend(definition.ManaCost))
            return Fail(slot, SkillUseFailure.NotEnoughMana);

        if (definition.CastSettings.CooldownStartMode ==
            SkillCooldownStartMode.OnCastStart)
        {
            runtime.StartCooldown();
        }

        _activeSkill = runtime;
        _activeContext = context;
        _activeSlot = slot;
        _activeSkillReleased = false;
        _channelManaAccumulator = 0f;

        SkillStarted?.Invoke(slot, runtime);
        return SkillUseFailure.None;
    }

    public void ReleaseActiveSkill()
    {
        if (_activeSkill == null || _activeSkillReleased)
            return;

        _activeDeliveryHandle =
            _activeSkill.Definition.Delivery.Deliver(_activeContext);

        _activeSkillReleased = true;
        SkillReleased?.Invoke(_activeSlot, _activeSkill);
    }

    public void FinishActiveSkill()
    {
        if (_activeSkill == null)
            return;

        SkillRuntime finishedSkill = _activeSkill;
        int finishedSlot = _activeSlot;

        StopActiveDelivery();

        if (finishedSkill.Definition.CastSettings.CooldownStartMode ==
            SkillCooldownStartMode.OnSkillEnd)
        {
            finishedSkill.StartCooldown();
        }

        ClearActiveSkill();
        SkillFinished?.Invoke(finishedSlot, finishedSkill);
    }

    public void CancelActiveSkill(bool force = false)
    {
        if (_activeSkill == null)
            return;

        if (!force &&
            !_activeSkill.Definition.CastSettings.CanBeInterrupted)
        {
            return;
        }

        SkillRuntime canceledSkill = _activeSkill;
        StopActiveDelivery();

        if (canceledSkill.Definition.CastSettings.CooldownStartMode ==
            SkillCooldownStartMode.OnSkillEnd)
        {
            canceledSkill.StartCooldown();
        }

        ClearActiveSkill();
    }

    public bool TryMaintainChannel(float deltaTime)
    {
        if (_activeSkill == null)
            return false;

        float costPerSecond =
            _activeSkill.Definition.ManaCostPerSecond;

        if (costPerSecond <= 0f)
            return true;

        _channelManaAccumulator += costPerSecond * deltaTime;
        int wholeManaCost = Mathf.FloorToInt(_channelManaAccumulator);

        if (wholeManaCost <= 0)
            return true;

        if (!_owner.Mana.TrySpend(wholeManaCost))
            return false;

        _channelManaAccumulator -= wholeManaCost;
        return true;
    }

    public void Dispose()
    {
        foreach (PassiveSkillRuntime passive in _passiveSkills)
            passive.Dispose();

        _passiveSkills.Clear();
        StopActiveDelivery();
        ClearActiveSkill();
    }

    private SkillUseFailure Fail(int slot, SkillUseFailure reason)
    {
        SkillFailed?.Invoke(slot, reason);
        return reason;
    }

    private void ClearActiveSkill()
    {
        _activeSkill = null;
        _activeContext = null;
        _activeSlot = -1;
        _activeSkillReleased = false;
        _channelManaAccumulator = 0f;
        _activeDeliveryHandle = null;
    }

    private void StopActiveDelivery()
    {
        _activeDeliveryHandle?.Stop();
        _activeDeliveryHandle = null;
    }

    private bool HasPassiveRuntime(SkillRuntime runtime)
    {
        foreach (PassiveSkillRuntime passive in _passiveSkills)
        {
            if (ReferenceEquals(passive.Definition, runtime.Definition))
                return true;
        }

        return false;
    }

    private static bool IsValidSlot(int slot)
    {
        return slot >= 0 && slot < PlayerManager.SkillBarSlotCount;
    }
}
