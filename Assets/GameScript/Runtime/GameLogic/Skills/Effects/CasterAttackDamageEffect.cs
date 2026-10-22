using UnityEngine;

[CreateAssetMenu(
    fileName = "CasterAttackDamageEffect",
    menuName = "ARPG/Skills/Effects/Caster Attack Damage")]
public sealed class CasterAttackDamageEffect : SkillEffect
{
    [SerializeField, Min(0f)] private float attackMultiplier = 1f;
    [SerializeField] private int flatBonus;

    public override void Apply(SkillHitContext context)
    {
        int attackDamage = context.Cast.AttackDamageSnapshot;
        float effectiveMultiplier = context.Cast.Build.GetNumeric(
            SkillNumericStat.AttackMultiplier,
            attackMultiplier);
        int effectiveFlatBonus = context.Cast.Build.GetInt(
            SkillNumericStat.FlatDamage,
            flatBonus);
        int finalDamage = Mathf.Max(
            0,
            Mathf.RoundToInt(attackDamage * effectiveMultiplier) +
                effectiveFlatBonus);

        foreach (SkillTarget target in context.Targets)
            target.Damageable?.TakeDamage(finalDamage);
    }
}
