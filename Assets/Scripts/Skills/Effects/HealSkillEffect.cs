using UnityEngine;

[CreateAssetMenu(
    fileName = "HealEffect",
    menuName = "ARPG/Skills/Effects/Heal")]
public sealed class HealSkillEffect : SkillEffect
{
    [SerializeField, Min(0)] private int amount = 10;

    public override void Apply(SkillHitContext context)
    {
        int finalAmount = context.Cast.Build.GetInt(
            SkillNumericStat.Healing,
            amount);

        foreach (SkillTarget target in context.Targets)
            target.Health?.Restore(finalAmount);
    }
}
