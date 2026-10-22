using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public sealed class PlayerManager : MonoBehaviour, IDamageable, IBuffReceiver
{
    public const int SkillBarSlotCount = (int)SkillSlot.Skill5 + 1;

    [Header("角色配置")]
    [SerializeField] private CharacterStatsData baseStats;

    [Header("技能释放")]
    [SerializeField] private Transform attackPoint;

    [Header("技能")]
    [FormerlySerializedAs("equippedSkills")]
    [SerializeField] private List<SkillDefinition> skillBar =
        new List<SkillDefinition>(SkillBarSlotCount);
    [SerializeField] private EnemyTargetSelector skillTargetSelector;

    [Header("技能成长")]
    [SerializeField] private SkillTreeDefinition skillTree;
    [SerializeField, Min(0)] private int initialSkillPoints;

    public event Action<int, SkillDefinition> SkillBarChanged;

    private Vector2 _moveInput;

    public CharacterStatsData BaseStats => baseStats;
    public Transform AttackPoint => attackPoint;
    public Health Health { get; private set; }
    public CharacterStatsRuntime Stats { get; private set; }
    public BuffController Buffs { get; private set; }
    public ResourcePool Mana { get; private set; }
    public PlayerSkillProgression SkillProgression { get; private set; }
    
    internal PlayerInputHandler Input { get; private set; }
    internal PlayerMovement Movement { get; private set; }
    internal PlayerAnimator Animation { get; private set; }
    internal PlayerSkillController Skills { get; private set; }
    public PlayerSkillCollection SkillCollection { get; private set; }
    internal PlayerNormalState NormalState { get; private set; }
    internal PlayerSkillState SkillState { get; private set; }
    internal PlayerDeadState DeadState { get; private set; }

    private StateMachine<PlayerManager> _stateMachine;

    private void OnValidate()
    {
        EnsureSkillBarShape();
    }
    
    private void Awake()
    {
        EnsureSkillBarShape();

        if (baseStats == null)
        {
            Debug.LogError(
                "PlayerManager 没有配置角色属性 SO。",
                this);

            enabled = false;
            return;
        }

        Stats = new CharacterStatsRuntime(baseStats);
        Health = new Health(baseStats.MaxHealth);
        Mana = new ResourcePool(baseStats.MaxMana);
        Buffs = new BuffController(this, Stats);

        Input = new PlayerInputHandler();

        Movement = new PlayerMovement(
            GetComponent<Rigidbody2D>(),
            transform,
            () => Stats.GetValue(CharacterStat.MoveSpeed));

        Animation = new PlayerAnimator(
            GetComponentInChildren<Animator>());

        if (skillTargetSelector == null)
            skillTargetSelector = FindObjectOfType<EnemyTargetSelector>();

        SkillCollection = new PlayerSkillCollection();
        IEnumerable<SkillDefinition> compatibilityStartingSkills =
            skillTree == null ? skillBar : null;
        SkillProgression = new PlayerSkillProgression(
            skillTree,
            SkillCollection,
            initialSkillPoints,
            compatibilityStartingSkills);
        SkillCollection.SkillUnregistered += OnSkillUnregistered;
        RemoveUnavailableSkillsFromBar();

        Skills = new PlayerSkillController(
            this,
            skillTargetSelector,
            SkillCollection,
            SkillProgression);

        _stateMachine = new StateMachine<PlayerManager>();

        NormalState = new PlayerNormalState(this, _stateMachine);
        SkillState = new PlayerSkillState(this, _stateMachine);
        DeadState = new PlayerDeadState(this, _stateMachine);

        Health.Died += OnDied;
        _stateMachine.ChangeState(NormalState);
    }

    private void OnEnable()
    {
        Input?.Enable();
    }

    private void Update()
    {
        Buffs?.Tick(Time.deltaTime);
        Skills?.Tick(Time.deltaTime);
        _stateMachine.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    private void OnDisable()
    {
        Input?.Disable();
        Movement?.Stop();
    }

    private void OnDestroy()
    {
        if (Health != null)
            Health.Died -= OnDied;

        if (SkillCollection != null)
            SkillCollection.SkillUnregistered -= OnSkillUnregistered;

        Input?.Dispose();
        Skills?.Dispose();
        Buffs?.Clear();
    }

    public void TakeDamage(int damage)
    {
        if (Buffs != null && Buffs.HasTag(BuffTag.Invulnerable))
            return;

        Health.TakeDamage(damage);
    }

    public bool TryGrantSkillPoints(
        int amount,
        SkillPointChangeReason reason = SkillPointChangeReason.Reward)
    {
        return SkillProgression != null &&
            SkillProgression.TryGrantPoints(amount, reason);
    }

    public string ExportCharacterJson(bool prettyPrint = true)
    {
        var data = new CharacterSaveData
        {
            skillProgress = SkillProgression != null
                ? SkillProgression.ExportSaveData()
                : new SkillProgressSaveData()
        };

        return JsonUtility.ToJson(data, prettyPrint);
    }

    public bool TryImportCharacterJson(string json)
    {
        if (SkillProgression == null || string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            CharacterSaveData data =
                JsonUtility.FromJson<CharacterSaveData>(json);
            if (data == null)
                return false;

            SkillProgression.ImportSaveData(data.skillProgress);
            RemoveUnavailableSkillsFromBar();
            return true;
        }
        catch (ArgumentException exception)
        {
            Debug.LogWarning(
                $"角色存档 JSON 无法读取：{exception.Message}",
                this);
            return false;
        }
    }

    public PlayerSkillEntry GetSkillEntry(SkillSlot slot)
    {
        return Skills?.GetSkillEntry((int)slot);
    }

    public SkillDefinition GetSkillAt(SkillSlot slot)
    {
        int slotIndex = (int)slot;
        return IsValidSlot(slotIndex) ? skillBar[slotIndex] : null;
    }

    public bool TrySetSkill(SkillSlot slot, SkillDefinition definition)
    {
        int slotIndex = (int)slot;

        if (!IsConfigurableSlot(slotIndex) ||
            !CanPlaceSkill(slotIndex, definition))
        {
            return false;
        }

        if (ReferenceEquals(skillBar[slotIndex], definition))
            return true;

        skillBar[slotIndex] = definition;
        SkillBarChanged?.Invoke(slotIndex, definition);
        return true;
    }

    public bool TrySwapSkill(SkillSlot first, SkillSlot second)
    {
        int firstIndex = (int)first;
        int secondIndex = (int)second;

        if (!IsConfigurableSlot(firstIndex) ||
            !IsConfigurableSlot(secondIndex) ||
            firstIndex == secondIndex)
        {
            return false;
        }

        SkillDefinition firstSkill = skillBar[firstIndex];
        skillBar[firstIndex] = skillBar[secondIndex];
        skillBar[secondIndex] = firstSkill;

        SkillBarChanged?.Invoke(firstIndex, skillBar[firstIndex]);
        SkillBarChanged?.Invoke(secondIndex, skillBar[secondIndex]);
        return true;
    }

    public bool ClearSkill(SkillSlot slot)
    {
        return TrySetSkill(slot, null);
    }

    public void PressSkillSlot(SkillSlot slot)
    {
        if (Input == null)
            return;

        Input.SetUISkillSlotHeld(slot, true);
        TryUseSkillSlot(slot);
    }

    public void ReleaseSkillSlot(SkillSlot slot)
    {
        Input?.SetUISkillSlotHeld(slot, false);
    }

    internal bool TryUseSkillSlot(int slot)
    {
        if (_stateMachine == null ||
            !ReferenceEquals(_stateMachine.CurrentState, NormalState) ||
            Skills.TryBeginUse(slot) != SkillUseFailure.None)
        {
            return false;
        }

        _stateMachine.ChangeState(SkillState);
        return true;
    }

    internal bool TryUseSkillSlot(SkillSlot slot)
    {
        return TryUseSkillSlot((int)slot);
    }

    private void OnDied()
    {
        _stateMachine.ChangeState(DeadState);
    }

    internal void ReadMovementInput()
    {
        _moveInput = Input.MoveInput;
        Animation.SetMoveSpeed(_moveInput);
    }

    internal void ApplyMovement()
    {
        Movement.FixedTick(_moveInput);
    }

    internal void StopMovement()
    {
        _moveInput = Vector2.zero;
        Movement.Stop();
        Animation.SetMoveSpeed(Vector2.zero);
    }

    public void AttackHit()
    {
        SkillCastPoint();
    }

    public void EndAttack()
    {
        EndSkill();
    }

    public void SkillCastPoint()
    {
        if (_stateMachine.CurrentState is PlayerSkillState skillState)
            skillState.ReleaseSkill();
    }

    public void EndSkill()
    {
        if (_stateMachine.CurrentState is PlayerSkillState skillState)
            skillState.FinishSkill();
    }

    private void EnsureSkillBarShape()
    {
        if (skillBar == null)
            skillBar = new List<SkillDefinition>(SkillBarSlotCount);

        while (skillBar.Count < SkillBarSlotCount)
            skillBar.Add(null);

        if (skillBar.Count > SkillBarSlotCount)
        {
            skillBar.RemoveRange(
                SkillBarSlotCount,
                skillBar.Count - SkillBarSlotCount);
        }
    }

    private bool CanPlaceSkill(int targetSlot, SkillDefinition definition)
    {
        if (definition == null)
            return true;

        PlayerSkillEntry entry = SkillCollection?.Get(definition);

        if (entry == null)
            return false;

        for (int i = 0; i < skillBar.Count; i++)
        {
            if (i != targetSlot && ReferenceEquals(skillBar[i], definition))
                return false;
        }

        return true;
    }

    private void OnSkillUnregistered(PlayerSkillEntry entry)
    {
        if (entry == null)
            return;

        for (int i = 0; i < skillBar.Count; i++)
        {
            if (!ReferenceEquals(skillBar[i], entry.Definition))
                continue;

            skillBar[i] = null;
            SkillBarChanged?.Invoke(i, null);
        }
    }

    private void RemoveUnavailableSkillsFromBar()
    {
        if (SkillCollection == null)
            return;

        for (int i = 0; i < skillBar.Count; i++)
        {
            SkillDefinition definition = skillBar[i];
            if (definition == null || SkillCollection.Contains(definition))
                continue;

            skillBar[i] = null;
            SkillBarChanged?.Invoke(i, null);
        }
    }

    private static bool IsValidSlot(int slot)
    {
        return slot >= 0 && slot < SkillBarSlotCount;
    }

    private static bool IsConfigurableSlot(int slot)
    {
        return IsValidSlot(slot) && slot != (int)SkillSlot.BasicAttack;
    }
}
