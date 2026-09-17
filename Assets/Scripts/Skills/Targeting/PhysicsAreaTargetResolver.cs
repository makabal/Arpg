using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "AreaTargeting",
    menuName = "ARPG/Skills/Targeting/Physics Area")]
public sealed class PhysicsAreaTargetResolver : SkillTargetResolver
{
    [SerializeField] private SkillTargetType targetType =
        SkillTargetType.DirectionalArea;
    [SerializeField] private SkillAreaOrigin origin =
        SkillAreaOrigin.Caster;
    [SerializeField] private SkillAreaShape shape =
        SkillAreaShape.Circle;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField, Min(0.01f)] private float radius = 2f;
    [SerializeField, Min(0.01f)] private float length = 3f;
    [SerializeField, Min(0.01f)] private float width = 1f;
    [SerializeField, Range(1f, 360f)] private float angle = 90f;
    [SerializeField, Min(0)] private int maxTargets;

    public override SkillTargetType TargetType => targetType;

    public override SkillUseFailure Resolve(
        in SkillTargetRequest request,
        List<SkillTarget> results)
    {
        Vector2 casterPosition = request.Caster.transform.position;
        Vector2 areaOrigin = origin == SkillAreaOrigin.AimPosition
            ? request.AimPosition
            : casterPosition;

        Collider2D[] hits = FindHits(
            casterPosition,
            areaOrigin,
            request.Direction);

        var added = new HashSet<IDamageable>();

        foreach (Collider2D hit in hits)
        {
            if (!SkillTarget.TryCreate(hit, out SkillTarget target) ||
                target.Damageable == null ||
                !added.Add(target.Damageable))
            {
                continue;
            }

            results.Add(target);

            if (maxTargets > 0 && results.Count >= maxTargets)
                break;
        }

        return results.Count > 0
            ? SkillUseFailure.None
            : SkillUseFailure.NoValidTarget;
    }

    private Collider2D[] FindHits(
        Vector2 casterPosition,
        Vector2 areaOrigin,
        Vector2 direction)
    {
        if (shape == SkillAreaShape.Line)
        {
            Vector2 center = casterPosition + direction * (length * 0.5f);
            float rotation = Mathf.Atan2(direction.y, direction.x) *
                Mathf.Rad2Deg;

            return Physics2D.OverlapBoxAll(
                center,
                new Vector2(length, width),
                rotation,
                targetLayers);
        }

        float searchRadius = shape == SkillAreaShape.Cone
            ? length
            : radius;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            areaOrigin,
            searchRadius,
            targetLayers);

        if (shape != SkillAreaShape.Cone)
            return hits;

        var filtered = new List<Collider2D>();

        foreach (Collider2D hit in hits)
        {
            Vector2 toTarget =
                (Vector2)hit.bounds.center - casterPosition;

            if (toTarget.sqrMagnitude <= length * length &&
                Vector2.Angle(direction, toTarget) <= angle * 0.5f)
            {
                filtered.Add(hit);
            }
        }

        return filtered.ToArray();
    }
}
