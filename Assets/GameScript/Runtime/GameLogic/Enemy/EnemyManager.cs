using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public sealed class EnemyManager : MonoBehaviour, IDamageable, IBuffReceiver
{
    [Header("基本信息")]
    [SerializeField] private string displayName = "Enemy";
    
    [Header("属性")]
    [SerializeField] private HealthMode healthMode = HealthMode.Normal;
    [SerializeField, Min(1)] private int maxHealth = 100;

    public string DisplayName => displayName;
    public Health Health { get; private set; }
    public BuffController Buffs { get; private set; }
    internal EnemyAnimationController Animation { get; private set; }
    internal EnemyIdleState IdleState { get; private set; }
    internal EnemyHitState HitState { get; private set; }

    private StateMachine<EnemyManager> _stateMachine;

    private void Awake()
    {
        Health = new Health(maxHealth, healthMode);
        Buffs = new BuffController(this);
        Animation = new EnemyAnimationController(GetComponent<Animator>());

        _stateMachine = new StateMachine<EnemyManager>();
        IdleState = new EnemyIdleState(this, _stateMachine);
        HitState = new EnemyHitState(this, _stateMachine);

        Health.Damaged += OnDamaged;
        Health.Died += OnDied;
        _stateMachine.ChangeState(IdleState);
    }

    private void Update()
    {
        Buffs?.Tick(Time.deltaTime);
        _stateMachine.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
    }

    private void OnDestroy()
    {
        Health.Damaged -= OnDamaged;
        Health.Died -= OnDied;
        Buffs?.Clear();
    }

    public void TakeDamage(int damage)
    {
        if (Buffs != null && Buffs.HasTag(BuffTag.Invulnerable))
            return;

        Health.TakeDamage(damage);
    }

    private void OnDamaged(int damage)
    {
        _stateMachine.ChangeState(HitState);
    }

    private void OnDied()
    {
        // 后续在这里接死亡动画、掉落和销毁逻辑。
    }

    public void EndHitAnimation()
    {
        if (_stateMachine.CurrentState is EnemyHitState hitState)
            hitState.FinishHit();
    }
}
