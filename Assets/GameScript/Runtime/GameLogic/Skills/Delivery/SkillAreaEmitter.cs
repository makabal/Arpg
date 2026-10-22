using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class SkillAreaEmitter : MonoBehaviour, ISkillDeliveryHandle
{
    private SkillCastContext _context;
    private SkillTargetResolver _targetResolver;
    private SkillAreaLifetimeMode _lifetimeMode;
    private float _remainingDuration;
    private float _tickInterval;
    private float _nextTickTime;
    private bool _followCaster;
    private bool _initialized;

    public void Initialize(
        SkillCastContext context,
        SkillTargetResolver targetResolver,
        SkillAreaLifetimeMode lifetimeMode,
        float duration,
        float tickInterval,
        bool applyImmediately,
        bool followCaster)
    {
        _context = context;
        _targetResolver = targetResolver;
        _lifetimeMode = lifetimeMode;
        _remainingDuration = Mathf.Max(0.01f, duration);
        _tickInterval = Mathf.Max(0.01f, tickInterval);
        _followCaster = followCaster;
        _initialized = true;

        UpdatePosition();

        if (applyImmediately)
        {
            ApplyTick();
            _nextTickTime = _tickInterval;
        }
        else
        {
            _nextTickTime = _tickInterval;
        }
    }

    private void Update()
    {
        if (!_initialized)
            return;

        PlayerManager caster = _context.Caster;

        if (caster == null ||
            !caster.isActiveAndEnabled ||
            caster.Health == null ||
            caster.Health.IsDead)
        {
            Destroy(gameObject);
            return;
        }

        UpdatePosition();

        float deltaTime = Time.deltaTime;
        if (_lifetimeMode == SkillAreaLifetimeMode.FixedDuration)
            _remainingDuration -= deltaTime;

        _nextTickTime -= deltaTime;

        while (IsAlive() && _nextTickTime <= 0f)
        {
            ApplyTick();
            _nextTickTime += _tickInterval;
        }

        if (!IsAlive())
            Destroy(gameObject);
    }

    private void ApplyTick()
    {
        var targets = new List<SkillTarget>();
        var request = new SkillTargetRequest(_context);

        if (_targetResolver.Resolve(request, targets) !=
            SkillUseFailure.None)
        {
            return;
        }

        var hit = new SkillHitContext(
            _context,
            targets,
            transform.position,
            Vector2.zero);

        SkillExecution.ApplyEffects(hit);
    }

    private void UpdatePosition()
    {
        if (_followCaster && _context.Caster != null)
        {
            transform.position = _context.Caster.transform.position;
        }
        else
        {
            transform.position = _context.Aim.Position;
        }
    }

    public void Stop()
    {
        if (this != null)
            Destroy(gameObject);
    }

    private bool IsAlive()
    {
        return _lifetimeMode == SkillAreaLifetimeMode.UntilSkillEnds ||
            _remainingDuration > 0f;
    }
}
