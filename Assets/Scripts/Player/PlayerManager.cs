using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public sealed class PlayerManager : MonoBehaviour, IDamageable
{

    [Header("角色配置")]
    [SerializeField] private CharacterStatsData baseStats;

    [Header("攻击")]
    [SerializeField] private Transform attackPoint;
    [SerializeField, Min(0f)] private float attackRadius = 0.8f;
    [SerializeField] private LayerMask enemyLayer;

    private Vector2 _moveInput;

    public CharacterStatsData BaseStats => baseStats;
    public Health Health { get; private set; }
    
    public ResourcePool Mana { get; private set; }
    
    internal PlayerInputHandler Input { get; private set; }
    internal PlayerMovement Movement { get; private set; }
    internal PlayerAnimator Animation { get; private set; }
    internal PlayerCombat Combat { get; private set; }
    internal PlayerNormalState NormalState { get; private set; }
    internal PlayerAttackState AttackState { get; private set; }
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

        Combat = new PlayerCombat(
            transform,
            attackPoint,
            attackRadius,
            enemyLayer,
            baseStats.AttackDamage);

        _stateMachine = new StateMachine<PlayerManager>();

        NormalState = new PlayerNormalState(this, _stateMachine);
        AttackState = new PlayerAttackState(this, _stateMachine);
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
        if (_stateMachine.CurrentState is PlayerAttackState attackState)
            attackState.AttackHit();
    }

    public void EndAttack()
    {
        if (_stateMachine.CurrentState is PlayerAttackState attackState)
            attackState.FinishAttack();
    }
}
