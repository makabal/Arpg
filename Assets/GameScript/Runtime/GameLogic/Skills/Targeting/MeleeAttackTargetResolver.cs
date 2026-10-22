using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "MeleeAttackTargeting",
    menuName = "ARPG/Skills/Targeting/Melee Attack")]
public sealed class MeleeAttackTargetResolver : SkillTargetResolver
{
    [SerializeField, Min(0.01f)] private float radius = 0.8f;
    [SerializeField] private LayerMask targetLayers;

    public override SkillTargetType TargetType =>
        SkillTargetType.DirectionalArea;

    public override SkillUseFailure Resolve(
        in SkillTargetRequest request,
        List<SkillTarget> results)
    {
        Transform attackPoint = request.Caster.AttackPoint;
        Vector2 center = attackPoint != null
            ? attackPoint.position
            : request.Caster.transform.position;

        float effectiveRadius = request.Build.GetNumeric(
            SkillNumericStat.AreaRadius,
            radius);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            center,
            effectiveRadius,
            targetLayers);

        var added = new HashSet<IDamageable>();
        bool facingRight = request.Direction.x >= 0f;

        foreach (Collider2D hit in hits)
        {
            float targetX = hit.bounds.center.x;
            bool attackPointHasNotPassedTarget = facingRight
                ? center.x <= targetX
                : center.x >= targetX;

            if (!attackPointHasNotPassedTarget ||
                !SkillTarget.TryCreate(hit, out SkillTarget target) ||
                target.Damageable == null ||
                !added.Add(target.Damageable))
            {
                continue;
            }

            results.Add(target);
        }

        // 近战挥空也是一次合法释放，不要求必须命中目标。
        return SkillUseFailure.None;
    }
}
