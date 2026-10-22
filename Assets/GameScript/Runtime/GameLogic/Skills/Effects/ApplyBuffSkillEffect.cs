using UnityEngine;

[CreateAssetMenu(
    fileName = "ApplyBuffEffect",
    menuName = "ARPG/Skills/Effects/Apply Buff")]
public sealed class ApplyBuffSkillEffect : SkillEffect
{
    [SerializeField] private BuffDefinition buff;

    public override void Apply(SkillHitContext context)
    {
        if (buff == null)
            return;

        float durationMultiplier = context.Cast.Build.GetNumeric(
            SkillNumericStat.BuffDurationMultiplier,
            1f);
        float potency = context.Cast.Build.GetNumeric(
            SkillNumericStat.BuffPotencyMultiplier,
            1f);

        foreach (SkillTarget target in context.Targets)
        {
            IBuffReceiver receiver = target.Owner as IBuffReceiver;
            if (receiver == null || receiver.Buffs == null)
                continue;

            receiver.Buffs.Apply(new BuffApplication(
                buff,
                context.Cast.Caster,
                durationMultiplier,
                potency));
        }
    }
}
