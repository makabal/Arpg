using System;
using UnityEngine;

[Serializable]
public sealed class SkillUpgradeRequirement
{
    [SerializeField] private string nodeId;
    [SerializeField, Min(1)] private int requiredRank = 1;

    public string NodeId => nodeId;
    public int RequiredRank => Mathf.Max(1, requiredRank);
}

[CreateAssetMenu(
    fileName = "SkillUpgradeNode",
    menuName = "ARPG/Skills/Upgrades/Upgrade Node")]
public sealed class SkillUpgradeNodeDefinition : ScriptableObject
{
    [Header("节点")]
    [SerializeField] private string nodeId;
    [SerializeField] private SkillDefinition skill;
    [SerializeField, Min(1)] private int maxRank = 1;
    [SerializeField, Min(0)] private int pointCost = 1;
    [SerializeField] private bool unlocksSkill;

    [Header("分支")]
    [SerializeField] private SkillUpgradeRequirement[] prerequisites;
    [Tooltip("相同且非空的组只能选择一个节点。")]
    [SerializeField] private string exclusiveGroupId;

    [Header("修改")]
    [SerializeField] private SkillUpgradeModifier[] modifiers;

    public string NodeId => nodeId;
    public SkillDefinition Skill => skill;
    public int MaxRank => Mathf.Max(1, maxRank);
    public int PointCost => Mathf.Max(0, pointCost);
    public bool UnlocksSkill => unlocksSkill;
    public SkillUpgradeRequirement[] Prerequisites =>
        prerequisites ?? Array.Empty<SkillUpgradeRequirement>();
    public string ExclusiveGroupId => exclusiveGroupId;
    public SkillUpgradeModifier[] Modifiers =>
        modifiers ?? Array.Empty<SkillUpgradeModifier>();
}
