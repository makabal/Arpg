using UnityEngine;

public readonly struct BuffApplication
{
    public BuffDefinition Definition { get; }
    public MonoBehaviour Source { get; }
    public float DurationMultiplier { get; }
    public float Potency { get; }

    public BuffApplication(
        BuffDefinition definition,
        MonoBehaviour source,
        float durationMultiplier = 1f,
        float potency = 1f)
    {
        Definition = definition;
        Source = source;
        DurationMultiplier = Mathf.Max(0f, durationMultiplier);
        Potency = Mathf.Max(0f, potency);
    }
}

public sealed class BuffInstance
{
    private readonly MonoBehaviour _target;
    private readonly CharacterStatsRuntime _stats;
    private float _tickTimer;

    public BuffDefinition Definition { get; }
    public MonoBehaviour Source { get; private set; }
    public float RemainingDuration { get; private set; }
    public int StackCount { get; private set; }
    public float Potency { get; private set; }
    public bool IsExpired => Definition == null ||
        (!Definition.IsPermanent && RemainingDuration <= 0f);

    internal BuffInstance(
        MonoBehaviour target,
        CharacterStatsRuntime stats,
        BuffApplication application)
    {
        _target = target;
        _stats = stats;
        Definition = application.Definition;
        Source = application.Source;
        Potency = application.Potency;
        StackCount = 1;
        RefreshDuration(application.DurationMultiplier);
        _tickTimer = Definition != null
            ? Definition.TickInterval
            : 0f;
    }

    internal void Activate()
    {
        RefreshStatModifiers();
        InvokeLifecycle(effect => effect.OnApply(CreateContext()));
    }

    internal void Refresh(BuffApplication application, bool addStack)
    {
        Source = application.Source;
        Potency = application.Potency;

        if (addStack && Definition != null)
            StackCount = Mathf.Min(StackCount + 1, Definition.MaxStacks);

        RefreshDuration(application.DurationMultiplier);
        RefreshStatModifiers();
    }

    internal void Tick(float deltaTime)
    {
        if (Definition == null || IsExpired)
            return;

        if (!Definition.IsPermanent)
            RemainingDuration -= Mathf.Max(0f, deltaTime);

        if (Definition.TickInterval <= 0f)
            return;

        _tickTimer -= Mathf.Max(0f, deltaTime);
        while (!IsExpired && _tickTimer <= 0f)
        {
            InvokeLifecycle(effect => effect.OnTick(CreateContext()));
            _tickTimer += Definition.TickInterval;
        }
    }

    internal void Deactivate()
    {
        _stats?.RemoveModifiers(this);
        InvokeLifecycle(effect => effect.OnRemove(CreateContext()));
    }

    private void RefreshDuration(float durationMultiplier)
    {
        RemainingDuration = Definition != null
            ? Definition.IsPermanent
                ? float.PositiveInfinity
                : Definition.Duration * Mathf.Max(0f, durationMultiplier)
            : 0f;
    }

    private void RefreshStatModifiers()
    {
        if (_stats == null || Definition == null)
            return;

        _stats.RemoveModifiers(this);

        foreach (BuffStatModifier modifier in Definition.StatModifiers)
        {
            if (modifier == null)
                continue;

            float stackMultiplier = modifier.ScaleWithStacks
                ? StackCount
                : 1f;

            _stats.AddModifier(
                this,
                modifier.Stat,
                modifier.Operation,
                modifier.Value * stackMultiplier * Potency);
        }
    }

    private BuffContext CreateContext()
    {
        return new BuffContext(_target, Source, this);
    }

    private void InvokeLifecycle(
        System.Action<BuffLifecycleEffect> callback)
    {
        if (Definition == null)
            return;

        foreach (BuffLifecycleEffect effect in Definition.LifecycleEffects)
        {
            if (effect != null)
                callback(effect);
        }
    }
}
