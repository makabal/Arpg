using UnityEngine;

[CreateAssetMenu(
    fileName = "SelfAim",
    menuName = "ARPG/Skills/Aiming/Self")]
public sealed class SelfAimResolver : SkillAimResolver
{
    public override SkillUseFailure Resolve(
        in SkillAimRequest request,
        out SkillAimData aim)
    {
        Vector2 direction = request.Caster.transform.localScale.x >= 0f
            ? Vector2.right
            : Vector2.left;

        aim = new SkillAimData(
            request.Caster.transform.position,
            direction);

        return SkillUseFailure.None;
    }
}
