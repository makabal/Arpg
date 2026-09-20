using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public sealed class PlayerManager : MonoBehaviour, IDamageable
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

    public event Action<int, SkillDefinition> SkillBarChanged;

    private Vector2 _moveInput;

    public CharacterStatsData BaseStats => baseStats;
    public Transform AttackPoint => attackPoint;
    public Health Health { get; private set; }
    
    public ResourcePool Mana { get; private set; }
    
    internal PlayerInputHandler Input { get; private set; }
    internal PlayerMovement Movement { get; private set; }
    internal PlayerAnimator Animation { get; private set; }
    internal PlayerSkillController Skills { get; private set; }
    public PlayerSkillCollection SkillCollection => Skills?.SkillCollection;
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

        Health = new Health(baseStats.MaxHealth);
        Mana = new ResourcePool(baseStats.MaxMana);

        Input = new PlayerInputHandler();

        Movement = new PlayerMovement(
            GetComponent<Rigidbody2D>(),
            transform,
            baseStats.MoveSpeed);

        Animation = new PlayerAnimator(
            GetComponentInChildren<Animator>());

        if (skillTargetSelector == null)
            skillTargetSelector = FindObjectOfType<EnemyTargetSelector>();

        Skills = new PlayerSkillController(
            this,
            skillBar,
            skillTargetSelector);

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

        Input?.Dispose();
        Skills?.Dispose();
    }

    public void TakeDamage(int damage)
    {
        Health.TakeDamage(damage);
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

        if (entry == null || !entry.IsUnlocked)
            return false;

        for (int i = 0; i < skillBar.Count; i++)
        {
            if (i != targetSlot && ReferenceEquals(skillBar[i], definition))
                return false;
        }

        return true;
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
