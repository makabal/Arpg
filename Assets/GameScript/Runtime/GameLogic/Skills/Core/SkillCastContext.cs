public sealed class SkillCastContext
{
    public PlayerManager Caster { get; }
    public SkillDefinition Definition { get; }
    public SkillBuildSnapshot Build { get; }
    public SkillAimData Aim { get; }
    public int AttackDamageSnapshot { get; }

    public SkillCastContext(
        PlayerManager caster,
        SkillDefinition definition,
        SkillAimData aim,
        SkillBuildSnapshot build = null)
    {
        Caster = caster;
        Definition = definition;
        Build = build ?? new SkillBuildBuilder(definition).Build();
        Aim = aim;
        AttackDamageSnapshot = caster.Stats != null
            ? caster.Stats.GetIntValue(CharacterStat.AttackDamage)
            : caster.BaseStats.AttackDamage;
    }
}
