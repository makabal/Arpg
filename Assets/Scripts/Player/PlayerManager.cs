using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public sealed class PlayerManager : MonoBehaviour, IDamageable
{

    [Header("角色配置")]
    [SerializeField] private CharacterStatsData baseStats;

    [Header("技能释放")]
    [SerializeField] private Transform attackPoint;

    [Header("技能")]
    [SerializeField] private SkillDefinition[] equippedSkills =
        new SkillDefinition[6];
    [SerializeField] private EnemyTargetSelector skillTargetSelector;

    private Vector2 _moveInput;

    public CharacterStatsData BaseStats => baseStats;
    public Transform AttackPoint => attackPoint;
    public Health Health { get; private set; }
    
    public ResourcePool Mana { get; private set; }
    
    internal PlayerInputHandler Input { get; private set; }
    internal PlayerMovement Movement { get; private set; }
    internal PlayerAnimator Animation { get; private set; }
    internal PlayerSkillController Skills { get; private set; }
    internal PlayerNormalState NormalState { get; private set; }
    internal PlayerSkillState SkillState { get; private set; }
    internal PlayerDeadState DeadState { get; private set; }

    private StateMachine<PlayerManager> _stateMachine;
    
    private void Awake()
    {
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
            equippedSkills,
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
}
