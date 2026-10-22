using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class PlayerStatusHud : MonoBehaviour
{
    [Header("数据来源")]
    [SerializeField] private PlayerManager player;

    [Header("角色信息")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image portraitImage;

    [Header("生命值")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    [Header("魔法值")]
    [SerializeField] private Slider manaSlider;
    [SerializeField] private TMP_Text manaText;

    private Health _health;
    private ResourcePool _mana;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError(
                "PlayerStatusHud 没有配置 PlayerManager。",
                this);

            enabled = false;
            return;
        }

        _health = player.Health;
        _mana = player.Mana;

        _health.Changed += RefreshHealth;
        _mana.Changed += RefreshMana;

        RefreshCharacterInfo();

        RefreshHealth(
            _health.CurrentHealth,
            _health.MaxHealth);

        RefreshMana(
            _mana.CurrentValue,
            _mana.MaxValue);
    }

    private void OnDestroy()
    {
        if (_health != null)
            _health.Changed -= RefreshHealth;

        if (_mana != null)
            _mana.Changed -= RefreshMana;
    }

    private void RefreshCharacterInfo()
    {
        CharacterStatsData data = player.BaseStats;

        nameText.text = data.DisplayName;

        if (data.Portrait != null)
            portraitImage.sprite = data.Portrait;
    }

    private void RefreshHealth(int current, int max)
    {
        healthSlider.normalizedValue = CalculateRatio(current, max);
        healthText.text = $"{current} / {max}";
    }

    private void RefreshMana(int current, int max)
    {
        manaSlider.normalizedValue = CalculateRatio(current, max);
        manaText.text = $"{current} / {max}";
    }

    private static float CalculateRatio(int current, int max)
    {
        if (max <= 0)
            return 0f;

        return Mathf.Clamp01((float)current / max);
    }
}