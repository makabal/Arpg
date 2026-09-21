using UnityEngine;

[CreateAssetMenu(
    fileName = "PersistentAreaDelivery",
    menuName = "ARPG/Skills/Delivery/Persistent Area")]
public sealed class PersistentAreaDelivery : SkillDelivery
{
    [SerializeField] private SkillTargetResolver targetResolver;
    [SerializeField] private SkillAreaLifetimeMode lifetimeMode;
    [SerializeField, Min(0.01f)] private float duration = 3f;
    [SerializeField, Min(0.01f)] private float tickInterval = 0.3f;
    [SerializeField] private bool applyImmediately = true;
    [SerializeField] private bool followCaster = true;

    public override ISkillDeliveryHandle Deliver(
        SkillCastContext context)
    {
        SkillTargetResolver resolver =
            context.Build.TargetResolverOverride ?? targetResolver;

        if (resolver == null)
        {
            Debug.LogWarning(
                $"技能 {context.Definition.DisplayName} 没有配置范围目标解析器。",
                context.Caster);
            return null;
        }

        var emitterObject = new GameObject(
            $"{context.Definition.DisplayName}_AreaEmitter");

        var emitter = emitterObject.AddComponent<SkillAreaEmitter>();
        float effectiveDuration = context.Build.GetNumeric(
            SkillNumericStat.AreaDuration,
            duration);
        float effectiveTickInterval = context.Build.GetNumeric(
            SkillNumericStat.TickInterval,
            tickInterval);
        emitter.Initialize(
            context,
            resolver,
            lifetimeMode,
            effectiveDuration,
            effectiveTickInterval,
            applyImmediately,
            followCaster);

        return lifetimeMode == SkillAreaLifetimeMode.UntilSkillEnds
            ? emitter
            : null;
    }
}
