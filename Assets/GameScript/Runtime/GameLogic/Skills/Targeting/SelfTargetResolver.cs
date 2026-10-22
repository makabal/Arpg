using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SelfTargeting",
    menuName = "ARPG/Skills/Targeting/Self")]
public sealed class SelfTargetResolver : SkillTargetResolver
{
    public override SkillTargetType TargetType => SkillTargetType.Self;

    public override SkillUseFailure Resolve(
        in SkillTargetRequest request,
        List<SkillTarget> results)
    {
        results.Add(new SkillTarget(
            request.Caster,
            request.Caster));

        return SkillUseFailure.None;
    }
}
