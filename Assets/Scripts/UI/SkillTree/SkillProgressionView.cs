using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class SkillProgressionView : MonoBehaviour
{
    [SerializeField] private PlayerManager player;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private Button resetButton;

    private PlayerSkillProgression _progression;

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<PlayerManager>();

        _progression = player != null
            ? player.SkillProgression
            : null;

        if (_progression == null)
        {
            Debug.LogError("SkillProgressionView 找不到玩家技能成长系统。", this);
            enabled = false;
            return;
        }

        _progression.PointsChanged += OnPointsChanged;

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetSkills);

        RefreshPoints();
    }

    private void OnDestroy()
    {
        if (_progression != null)
            _progression.PointsChanged -= OnPointsChanged;

        if (resetButton != null)
            resetButton.onClick.RemoveListener(ResetSkills);
    }

    private void OnPointsChanged(SkillPointChange change)
    {
        RefreshPoints();
    }

    private void RefreshPoints()
    {
        if (pointsText != null && _progression != null)
            pointsText.text = _progression.AvailablePoints.ToString();
    }

    private void ResetSkills()
    {
        _progression?.ResetAll();
    }
}
