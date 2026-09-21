using UnityEngine;

[CreateAssetMenu(
    fileName = "DamageEffect",
    menuName = "ARPG/Skills/Effects/Damage")]
public sealed class DamageSkillEffect : SkillEffect
{
    [SerializeField, Min(0)] private int damage = 10;

    public override void Apply(SkillHitContext context)
    {
        int finalDamage = context.Cast.Build.GetInt(
            SkillNumericStat.Damage,
            damage);

        foreach (SkillTarget target in context.Targets)
            target.Damageable?.TakeDamage(finalDamage);
    }
}
