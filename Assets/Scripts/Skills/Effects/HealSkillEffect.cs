using UnityEngine;

[CreateAssetMenu(
    fileName = "HealEffect",
    menuName = "ARPG/Skills/Effects/Heal")]
public sealed class HealSkillEffect : SkillEffect
{
    [SerializeField, Min(0)] private int amount = 10;

    public override void Apply(SkillHitContext context)
    {
        foreach (SkillTarget target in context.Targets)
            target.Health?.Restore(amount);
    }
}
