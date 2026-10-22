using UnityEngine;

[CreateAssetMenu(
    fileName = "StructureSkillUpgrade",
    menuName = "ARPG/Skills/Upgrades/Structure Modifier")]
public sealed class SkillStructureUpgradeModifier : SkillUpgradeModifier
{
    [Header("可选替换")]
    [SerializeField] private SkillAimResolver aimResolverOverride;
    [SerializeField] private SkillDelivery deliveryOverride;
    [SerializeField] private SkillTargetResolver targetResolverOverride;

    [Header("效果与条件")]
    [SerializeField] private SkillEffect[] addedEffects;
    [SerializeField] private SkillEffect[] removedEffects;
    [SerializeField] private SkillCondition[] addedConditions;

    [Header("表现")]
    [SerializeField] private bool overridePresentation;
    [SerializeField] private SkillPresentationData presentationOverride =
        new SkillPresentationData();

    public override void Apply(SkillBuildBuilder builder, int rank)
    {
        if (builder == null || rank <= 0)
            return;

        if (aimResolverOverride != null)
            builder.SetAimResolver(aimResolverOverride);

        if (deliveryOverride != null)
            builder.SetDelivery(deliveryOverride);

        if (targetResolverOverride != null)
            builder.SetTargetResolverOverride(targetResolverOverride);

        if (removedEffects != null)
        {
            foreach (SkillEffect effect in removedEffects)
                builder.RemoveEffect(effect);
        }

        if (addedEffects != null)
        {
            foreach (SkillEffect effect in addedEffects)
                builder.AddEffect(effect);
        }

        if (addedConditions != null)
        {
            foreach (SkillCondition condition in addedConditions)
                builder.AddCondition(condition);
        }

        if (overridePresentation)
            builder.SetPresentation(presentationOverride);
    }
}
