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
        if (targetResolver == null)
            return null;

        var targets = new List<SkillTarget>();
        var request = new SkillTargetRequest(context);

        if (targetResolver.Resolve(request, targets) !=
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
