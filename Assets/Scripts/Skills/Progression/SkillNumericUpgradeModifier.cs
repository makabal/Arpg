using UnityEngine;

[CreateAssetMenu(
    fileName = "NumericSkillUpgrade",
    menuName = "ARPG/Skills/Upgrades/Numeric Modifier")]
public sealed class SkillNumericUpgradeModifier : SkillUpgradeModifier
{
    [SerializeField] private SkillNumericStat stat;
    [SerializeField] private SkillNumericOperation operation;
    [SerializeField] private float valuePerRank;

    public override void Apply(SkillBuildBuilder builder, int rank)
    {
        if (builder == null || rank <= 0)
            return;

        float value = operation == SkillNumericOperation.Override
            ? valuePerRank
            : valuePerRank * rank;

        builder.AddNumericModifier(stat, operation, value);
    }
}
