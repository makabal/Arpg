using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class SkillUpgradeNodeView : MonoBehaviour
{
    [SerializeField] private PlayerManager player;
    [SerializeField] private SkillUpgradeNodeDefinition node;
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TMP_Text rankText;

    private PlayerSkillProgression _progression;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerManager>();

        _progression = player != null
            ? player.SkillProgression
            : null;

        if (_progression == null || node == null)
        {
            Debug.LogError("SkillUpgradeNodeView 缺少玩家或升级节点配置。", this);
            enabled = false;
            return;
        }

        _progression.PointsChanged += OnPointsChanged;
        _progression.NodeRankChanged += OnNodeRankChanged;

        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(Purchase);

        Refresh();
    }

    private void OnDestroy()
    {
        if (_progression != null)
        {
            _progression.PointsChanged -= OnPointsChanged;
            _progression.NodeRankChanged -= OnNodeRankChanged;
        }

        if (purchaseButton != null)
            purchaseButton.onClick.RemoveListener(Purchase);
    }

    private void Purchase()
    {
        if (_progression != null && node != null)
            _progression.TryPurchase(node.NodeId, out _);
    }

    private void OnPointsChanged(SkillPointChange change)
    {
        Refresh();
    }

    private void OnNodeRankChanged(
        SkillUpgradeNodeDefinition changedNode,
        int rank)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (_progression == null || node == null)
            return;

        int rank = _progression.GetNodeRank(node.NodeId);

        if (rankText != null)
            rankText.text = $"{rank} / {node.MaxRank}";

        if (purchaseButton != null)
        {
            purchaseButton.interactable =
                _progression.GetPurchaseFailure(node.NodeId) ==
                    SkillUpgradeFailure.None;
        }
    }
}
