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
        int finalDamage = Mathf.Max(
            0,
            Mathf.RoundToInt(attackDamage * attackMultiplier) + flatBonus);

        foreach (SkillTarget target in context.Targets)
            target.Damageable?.TakeDamage(finalDamage);
    }
}
