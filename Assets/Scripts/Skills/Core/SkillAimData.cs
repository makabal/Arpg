using UnityEngine;

public readonly struct SkillAimData
{
    public Vector2 Position { get; }
    public Vector2 Direction { get; }
    public EnemyManager LockedTarget { get; }

    public SkillAimData(
        Vector2 position,
        Vector2 direction,
        EnemyManager lockedTarget = null)
    {
        Position = position;
        Direction = direction.sqrMagnitude > 0.0001f
            ? direction.normalized
            : Vector2.right;
        LockedTarget = lockedTarget;
    }
}
