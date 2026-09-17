using UnityEngine;

[CreateAssetMenu(
    fileName = "SelectedTargetAim",
    menuName = "ARPG/Skills/Aiming/Selected Enemy")]
public sealed class SelectedTargetAimResolver : SkillAimResolver
{
    public override SkillUseFailure Resolve(
        in SkillAimRequest request,
        out SkillAimData aim)
    {
        EnemyManager target = request.SelectedEnemy;

        if (target == null || target.Health == null || target.Health.IsDead)
        {
            aim = default;
            return SkillUseFailure.NoValidTarget;
        }

        Vector2 casterPosition = request.Caster.transform.position;
        Vector2 targetPosition = target.transform.position;

        if (request.CastRange > 0f &&
            Vector2.Distance(casterPosition, targetPosition) >
                request.CastRange)
        {
            aim = default;
            return SkillUseFailure.OutOfRange;
        }

        aim = new SkillAimData(
            targetPosition,
            targetPosition - casterPosition,
            target);

        return SkillUseFailure.None;
    }
}
