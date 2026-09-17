using UnityEngine;

[CreateAssetMenu(
    fileName = "PointerAim",
    menuName = "ARPG/Skills/Aiming/Pointer Direction")]
public sealed class PointerAimResolver : SkillAimResolver
{
    [SerializeField] private bool clampPositionToCastRange;

    public override SkillUseFailure Resolve(
        in SkillAimRequest request,
        out SkillAimData aim)
    {
        Vector2 casterPosition = request.Caster.transform.position;
        Vector2 offset = request.PointerWorldPosition - casterPosition;

        if (offset.sqrMagnitude < 0.0001f)
        {
            offset = request.Caster.transform.localScale.x >= 0f
                ? Vector2.right
                : Vector2.left;
        }

        Vector2 position = request.PointerWorldPosition;

        if (clampPositionToCastRange &&
            request.CastRange > 0f &&
            offset.magnitude > request.CastRange)
        {
            position = casterPosition +
                offset.normalized * request.CastRange;
        }

        aim = new SkillAimData(position, offset);
        return SkillUseFailure.None;
    }
}
