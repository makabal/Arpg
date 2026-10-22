using System.Collections.Generic;
using UnityEngine;

public sealed class SkillHitContext
{
    public SkillCastContext Cast { get; }
    public IReadOnlyList<SkillTarget> Targets { get; }
    public Vector2 HitPoint { get; }
    public Vector2 HitNormal { get; }

    public SkillHitContext(
        SkillCastContext cast,
        IReadOnlyList<SkillTarget> targets,
        Vector2 hitPoint,
        Vector2 hitNormal)
    {
        Cast = cast;
        Targets = targets;
        HitPoint = hitPoint;
        HitNormal = hitNormal;
    }
}
