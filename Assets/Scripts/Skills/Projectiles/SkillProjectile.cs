using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class SkillProjectile : MonoBehaviour
{
    private readonly HashSet<IDamageable> _hitTargets =
        new HashSet<IDamageable>();

    private SkillCastContext _castContext;
    private LayerMask _targetLayers;
    private Rigidbody2D _rigidbody;
    private Vector2 _direction;
    private float _speed;
    private float _remainingLifetime;
    private int _maxHits;
    private bool _destroyWhenMaxHitsReached;
    private bool _initialized;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Initialize(
        SkillCastContext context,
        LayerMask targetLayers,
        float speed,
        float lifetime,
        int maxHits,
        bool destroyWhenMaxHitsReached)
    {
        _castContext = context;
        _targetLayers = targetLayers;
        _direction = context.Aim.Direction;
        _speed = speed;
        _remainingLifetime = lifetime;
        _maxHits = Mathf.Max(1, maxHits);
        _destroyWhenMaxHitsReached = destroyWhenMaxHitsReached;
        _initialized = true;

        if (_rigidbody != null)
            _rigidbody.velocity = _direction * _speed;
    }

    private void Update()
    {
        if (!_initialized)
            return;

        _remainingLifetime -= Time.deltaTime;

        if (_remainingLifetime <= 0f)
        {
            Destroy(gameObject);
            return;
        }

        if (_rigidbody == null)
        {
            transform.position +=
                (Vector3)(_direction * (_speed * Time.deltaTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_initialized ||
            _hitTargets.Count >= _maxHits ||
            (_targetLayers.value & (1 << other.gameObject.layer)) == 0 ||
            !SkillTarget.TryCreate(other, out SkillTarget target) ||
            target.Damageable == null ||
            ReferenceEquals(target.Damageable, _castContext.Caster) ||
            !_hitTargets.Add(target.Damageable))
        {
            return;
        }

        Vector2 hitPoint = other.ClosestPoint(transform.position);
        var targets = new[] { target };
        var hit = new SkillHitContext(
            _castContext,
            targets,
            hitPoint,
            Vector2.zero);

        SkillExecution.ApplyEffects(hit);

        if (_destroyWhenMaxHitsReached &&
            _hitTargets.Count >= _maxHits)
        {
            Destroy(gameObject);
        }
    }
}
