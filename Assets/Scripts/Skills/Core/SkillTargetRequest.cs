using UnityEngine;

public readonly struct SkillTargetRequest
{
    public PlayerManager Caster { get; }
    public EnemyManager SelectedEnemy { get; }
    public Vector2 AimPosition { get; }
    public Vector2 Direction { get; }
    public float CastRange { get; }
    public SkillBuildSnapshot Build { get; }

    public SkillTargetRequest(SkillCastContext context)
    {
        Caster = context.Caster;
        SelectedEnemy = context.Aim.LockedTarget;
        AimPosition = context.Aim.Position;
        Direction = context.Aim.Direction;
        Build = context.Build;
        CastRange = context.Build.GetNumeric(
            SkillNumericStat.CastRange,
            context.Definition.CastRange);
    }
}
