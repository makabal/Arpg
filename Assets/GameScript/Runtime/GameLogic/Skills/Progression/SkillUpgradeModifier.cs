using UnityEngine;

public enum SkillNumericStat
{
    ManaCost,
    ManaCostPerSecond,
    Cooldown,
    CastRange,
    CastEffectTime,
    CastTotalDuration,
    Damage,
    AttackMultiplier,
    FlatDamage,
    Healing,
    AreaRadius,
    AreaLength,
    AreaWidth,
    AreaAngle,
    AreaDuration,
    TickInterval,
    ProjectileSpeed,
    ProjectileLifetime,
    ProjectileMaxHits,
    BuffDurationMultiplier,
    BuffPotencyMultiplier
}

public enum SkillNumericOperation
{
    Add,
    Percent,
    Override
}

public abstract class SkillUpgradeModifier : ScriptableObject
{
    public abstract void Apply(SkillBuildBuilder builder, int rank);
}
