using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "InstantTargetDelivery",
    menuName = "ARPG/Skills/Delivery/Instant Target")]
public sealed class InstantTargetDelivery : SkillDelivery
{
    [SerializeField] private SkillTargetResolver targetResolver;

    public override ISkillDeliveryHandle Deliver(
        SkillCastContext context)
    {
        SkillTargetResolver resolver =
            context.Build.TargetResolverOverride ?? targetResolver;

        if (resolver == null)
            return null;

        var targets = new List<SkillTarget>();
        var request = new SkillTargetRequest(context);

        if (resolver.Resolve(request, targets) !=
            SkillUseFailure.None)
        {
            return null;
        }

        var hit = new SkillHitContext(
            context,
            targets,
            context.Aim.Position,
            Vector2.zero);

        SkillExecution.ApplyEffects(hit);
        return null;
    }
}
