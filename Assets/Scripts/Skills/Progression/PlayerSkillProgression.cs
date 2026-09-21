using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillUpgradeFailure
{
    None,
    InvalidNode,
    SkillNotOwned,
    MaxRank,
    MissingPrerequisite,
    ExclusiveBranchSelected,
    NotEnoughPoints
}

public enum SkillPointChangeReason
{
    Initial,
    Reward,
    Purchase,
    Reset,
    Load,
    Debug
}

public readonly struct SkillPointChange
{
    public int PreviousValue { get; }
    public int CurrentValue { get; }
    public int Delta => CurrentValue - PreviousValue;
    public SkillPointChangeReason Reason { get; }

    public SkillPointChange(
        int previousValue,
        int currentValue,
        SkillPointChangeReason reason)
    {
        PreviousValue = previousValue;
        CurrentValue = currentValue;
        Reason = reason;
    }
}

public sealed class PlayerSkillProgression
{
    private readonly SkillTreeDefinition _tree;
    private readonly PlayerSkillCollection _collection;
    private readonly Dictionary<string, int> _nodeRanks =
        new Dictionary<string, int>();
    private readonly HashSet<SkillDefinition> _baselineSkills =
        new HashSet<SkillDefinition>();

    public int AvailablePoints { get; private set; }
    public int TotalEarnedPoints { get; private set; }
    public SkillTreeDefinition Tree => _tree;

    public event Action<SkillPointChange> PointsChanged;
    public event Action<SkillDefinition> SkillBuildChanged;
    public event Action<SkillUpgradeNodeDefinition, int> NodeRankChanged;

    public PlayerSkillProgression(
        SkillTreeDefinition tree,
        PlayerSkillCollection collection,
        int initialPoints,
        IEnumerable<SkillDefinition> compatibilityStartingSkills = null)
    {
        _tree = tree;
        _collection = collection ??
            throw new ArgumentNullException(nameof(collection));

        AvailablePoints = Mathf.Max(0, initialPoints);
        TotalEarnedPoints = AvailablePoints;

        if (_tree != null)
        {
            foreach (SkillDefinition skill in _tree.StartingSkills)
                AddBaselineSkill(skill);
        }

        if (compatibilityStartingSkills != null)
        {
            foreach (SkillDefinition skill in compatibilityStartingSkills)
                AddBaselineSkill(skill);
        }
    }

    public int GetNodeRank(string nodeId)
    {
        return !string.IsNullOrWhiteSpace(nodeId) &&
            _nodeRanks.TryGetValue(nodeId, out int rank)
                ? rank
                : 0;
    }

    public bool TryGrantPoints(
        int amount,
        SkillPointChangeReason reason = SkillPointChangeReason.Reward)
    {
        if (amount <= 0 || AvailablePoints > int.MaxValue - amount)
            return false;

        int previous = AvailablePoints;
        AvailablePoints += amount;
        TotalEarnedPoints = Mathf.Max(
            TotalEarnedPoints,
            AvailablePoints + CalculateSpentPoints());
        PointsChanged?.Invoke(
            new SkillPointChange(previous, AvailablePoints, reason));
        return true;
    }

    public bool TryPurchase(
        string nodeId,
        out SkillUpgradeFailure failure)
    {
        SkillUpgradeNodeDefinition node =
            _tree != null ? _tree.FindNode(nodeId) : null;

        failure = GetPurchaseFailure(node);
        if (failure != SkillUpgradeFailure.None)
            return false;

        int previousPoints = AvailablePoints;
        AvailablePoints -= node.PointCost;

        int rank = GetNodeRank(node.NodeId) + 1;
        _nodeRanks[node.NodeId] = rank;

        if (node.UnlocksSkill && node.Skill != null)
            _collection.Register(node.Skill);

        PointsChanged?.Invoke(new SkillPointChange(
            previousPoints,
            AvailablePoints,
            SkillPointChangeReason.Purchase));
        NodeRankChanged?.Invoke(node, rank);
        SkillBuildChanged?.Invoke(node.Skill);
        return true;
    }

    public SkillUpgradeFailure GetPurchaseFailure(string nodeId)
    {
        SkillUpgradeNodeDefinition node =
            _tree != null ? _tree.FindNode(nodeId) : null;
        return GetPurchaseFailure(node);
    }

    public void ResetAll()
    {
        int previousPoints = AvailablePoints;
        int refund = CalculateSpentPoints();
        var changedSkills = new HashSet<SkillDefinition>();

        var changedNodes = new List<SkillUpgradeNodeDefinition>();

        if (_tree != null)
        {
            foreach (SkillUpgradeNodeDefinition node in _tree.Nodes)
            {
                if (node != null && GetNodeRank(node.NodeId) > 0)
                {
                    changedSkills.Add(node.Skill);
                    changedNodes.Add(node);
                }
            }
        }

        _nodeRanks.Clear();
        AvailablePoints = Mathf.Min(
            TotalEarnedPoints,
            AvailablePoints + refund);
        RebuildOwnedSkills();

        PointsChanged?.Invoke(new SkillPointChange(
            previousPoints,
            AvailablePoints,
            SkillPointChangeReason.Reset));

        foreach (SkillUpgradeNodeDefinition node in changedNodes)
            NodeRankChanged?.Invoke(node, 0);

        foreach (SkillDefinition skill in changedSkills)
            SkillBuildChanged?.Invoke(skill);
    }

    public SkillBuildSnapshot BuildSkill(SkillDefinition definition)
    {
        var builder = new SkillBuildBuilder(definition);

        if (_tree != null)
        {
            foreach (SkillUpgradeNodeDefinition node in _tree.Nodes)
            {
                if (node == null ||
                    !ReferenceEquals(node.Skill, definition))
                {
                    continue;
                }

                int rank = GetNodeRank(node.NodeId);
                if (rank <= 0)
                    continue;

                foreach (SkillUpgradeModifier modifier in node.Modifiers)
                    modifier?.Apply(builder, rank);
            }
        }

        return builder.Build();
    }

    public SkillProgressSaveData ExportSaveData()
    {
        var data = new SkillProgressSaveData
        {
            availablePoints = AvailablePoints,
            totalEarnedPoints = TotalEarnedPoints
        };

        foreach (KeyValuePair<string, int> pair in _nodeRanks)
        {
            if (pair.Value <= 0)
                continue;

            data.nodes.Add(new SkillNodeProgressData
            {
                nodeId = pair.Key,
                rank = pair.Value
            });
        }

        return data;
    }

    public void ImportSaveData(SkillProgressSaveData data)
    {
        int previousPoints = AvailablePoints;
        _nodeRanks.Clear();

        if (data == null)
        {
            AvailablePoints = 0;
            TotalEarnedPoints = 0;
        }
        else
        {
            AvailablePoints = Mathf.Max(0, data.availablePoints);
            TotalEarnedPoints = Mathf.Max(
                AvailablePoints,
                data.totalEarnedPoints);

            if (data.nodes != null && _tree != null)
            {
                foreach (SkillNodeProgressData savedNode in data.nodes)
                {
                    if (savedNode == null)
                        continue;

                    SkillUpgradeNodeDefinition node =
                        _tree.FindNode(savedNode.nodeId);
                    if (node == null)
                        continue;

                    _nodeRanks[node.NodeId] = Mathf.Clamp(
                        savedNode.rank,
                        0,
                        node.MaxRank);
                }
            }

            TotalEarnedPoints = Mathf.Max(
                TotalEarnedPoints,
                AvailablePoints + CalculateSpentPoints());
        }

        RebuildOwnedSkills();
        PointsChanged?.Invoke(new SkillPointChange(
            previousPoints,
            AvailablePoints,
            SkillPointChangeReason.Load));

        if (_tree != null)
        {
            var changedSkills = new HashSet<SkillDefinition>();
            foreach (SkillUpgradeNodeDefinition node in _tree.Nodes)
            {
                if (node == null)
                    continue;

                NodeRankChanged?.Invoke(
                    node,
                    GetNodeRank(node.NodeId));

                if (node.Skill != null)
                    changedSkills.Add(node.Skill);
            }

            foreach (SkillDefinition skill in changedSkills)
                SkillBuildChanged?.Invoke(skill);
        }
    }

    private SkillUpgradeFailure GetPurchaseFailure(
        SkillUpgradeNodeDefinition node)
    {
        if (node == null || string.IsNullOrWhiteSpace(node.NodeId))
            return SkillUpgradeFailure.InvalidNode;

        int currentRank = GetNodeRank(node.NodeId);
        if (currentRank >= node.MaxRank)
            return SkillUpgradeFailure.MaxRank;

        if (!node.UnlocksSkill &&
            node.Skill != null &&
            !_collection.Contains(node.Skill))
        {
            return SkillUpgradeFailure.SkillNotOwned;
        }

        foreach (SkillUpgradeRequirement requirement in node.Prerequisites)
        {
            if (requirement == null ||
                GetNodeRank(requirement.NodeId) < requirement.RequiredRank)
            {
                return SkillUpgradeFailure.MissingPrerequisite;
            }
        }

        if (!string.IsNullOrWhiteSpace(node.ExclusiveGroupId) &&
            HasOtherNodeInExclusiveGroup(node))
        {
            return SkillUpgradeFailure.ExclusiveBranchSelected;
        }

        return AvailablePoints >= node.PointCost
            ? SkillUpgradeFailure.None
            : SkillUpgradeFailure.NotEnoughPoints;
    }

    private bool HasOtherNodeInExclusiveGroup(
        SkillUpgradeNodeDefinition target)
    {
        if (_tree == null)
            return false;

        foreach (SkillUpgradeNodeDefinition node in _tree.Nodes)
        {
            if (node == null || ReferenceEquals(node, target))
                continue;

            if (node.ExclusiveGroupId == target.ExclusiveGroupId &&
                GetNodeRank(node.NodeId) > 0)
            {
                return true;
            }
        }

        return false;
    }

    private int CalculateSpentPoints()
    {
        int spent = 0;
        if (_tree == null)
            return spent;

        foreach (SkillUpgradeNodeDefinition node in _tree.Nodes)
        {
            if (node == null)
                continue;

            spent += node.PointCost * GetNodeRank(node.NodeId);
        }

        return spent;
    }

    private void AddBaselineSkill(SkillDefinition skill)
    {
        if (skill == null || !_baselineSkills.Add(skill))
            return;

        _collection.Register(skill);
    }

    private void RebuildOwnedSkills()
    {
        var desired = new HashSet<SkillDefinition>(_baselineSkills);

        if (_tree != null)
        {
            foreach (SkillUpgradeNodeDefinition node in _tree.Nodes)
            {
                if (node != null &&
                    node.UnlocksSkill &&
                    node.Skill != null &&
                    GetNodeRank(node.NodeId) > 0)
                {
                    desired.Add(node.Skill);
                }
            }
        }

        var toRemove = new List<SkillDefinition>();
        foreach (PlayerSkillEntry entry in _collection.Entries)
        {
            if (!desired.Contains(entry.Definition))
                toRemove.Add(entry.Definition);
        }

        foreach (SkillDefinition definition in toRemove)
            _collection.Unregister(definition);

        foreach (SkillDefinition definition in desired)
            _collection.Register(definition);
    }
}
