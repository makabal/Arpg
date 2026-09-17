public sealed class SkillCastContext
{
    public PlayerManager Caster { get; }
    public SkillDefinition Definition { get; }
    public SkillAimData Aim { get; }
    public int AttackDamageSnapshot { get; }

    public SkillCastContext(
        PlayerManager caster,
        SkillDefinition definition,
        SkillAimData aim)
    {
        Caster = caster;
        Definition = definition;
        Aim = aim;
        AttackDamageSnapshot = caster.BaseStats.AttackDamage;
    }
}
