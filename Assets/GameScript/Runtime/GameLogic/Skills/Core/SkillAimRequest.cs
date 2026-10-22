using UnityEngine;

public readonly struct SkillAimRequest
{
    public PlayerManager Caster { get; }
    public EnemyManager SelectedEnemy { get; }
    public Vector2 PointerWorldPosition { get; }
    public float CastRange { get; }

    public SkillAimRequest(
        PlayerManager caster,
        EnemyManager selectedEnemy,
        Vector2 pointerWorldPosition,
        float castRange)
    {
        Caster = caster;
        SelectedEnemy = selectedEnemy;
        PointerWorldPosition = pointerWorldPosition;
        CastRange = castRange;
    }
}
