using System;
using UnityEngine;

[CreateAssetMenu(
    fileName = "SkillTree",
    menuName = "ARPG/Skills/Skill Tree")]
public sealed class SkillTreeDefinition : ScriptableObject
{
    [SerializeField] private SkillDefinition[] startingSkills;
    [SerializeField] private SkillUpgradeNodeDefinition[] nodes;

    public SkillDefinition[] StartingSkills =>
        startingSkills ?? Array.Empty<SkillDefinition>();
    public SkillUpgradeNodeDefinition[] Nodes =>
        nodes ?? Array.Empty<SkillUpgradeNodeDefinition>();

    public SkillUpgradeNodeDefinition FindNode(string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return null;

        foreach (SkillUpgradeNodeDefinition node in Nodes)
        {
            if (node != null && node.NodeId == nodeId)
                return node;
        }

        return null;
    }

    private void OnValidate()
    {
        var ids = new System.Collections.Generic.HashSet<string>();

        foreach (SkillUpgradeNodeDefinition node in Nodes)
        {
            if (node == null || string.IsNullOrWhiteSpace(node.NodeId))
                continue;

            if (!ids.Add(node.NodeId))
            {
                Debug.LogError(
                    $"技能树 {name} 存在重复节点 ID：{node.NodeId}",
                    this);
            }
        }
    }
}
