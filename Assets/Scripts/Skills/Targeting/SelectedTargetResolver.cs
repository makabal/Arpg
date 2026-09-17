using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SelectedTargeting",
    menuName = "ARPG/Skills/Targeting/Selected Enemy")]
public sealed class SelectedTargetResolver : SkillTargetResolver
{
    public override SkillTargetType TargetType =>
        SkillTargetType.SelectedTarget;

    public override SkillUseFailure Resolve(
        in SkillTargetRequest request,
        List<SkillTarget> results)
    {
        EnemyManager enemy = request.SelectedEnemy;

        if (enemy == null || enemy.Health == null || enemy.Health.IsDead)
            return SkillUseFailure.NoValidTarget;

        float distance = Vector2.Distance(
            request.Caster.transform.position,
            enemy.transform.position);

        if (request.CastRange > 0f && distance > request.CastRange)
            return SkillUseFailure.OutOfRange;

        results.Add(new SkillTarget(enemy, enemy));
        return SkillUseFailure.None;
    }
}
