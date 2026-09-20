using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class SkillSlotView : MonoBehaviour
{
    [Header("槽位")]
    [SerializeField] private SkillSlot slot;

    [Header("显示")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownFill;
    [SerializeField] private TMP_Text hotkeyText;
    [SerializeField] private Button skillButton;

    private PlayerManager _player;
    private PlayerSkillEntry _entry;
    private SkillRuntime _runtime;

    public SkillSlot Slot => slot;

    private void Start()
    {
        _player = FindObjectOfType<PlayerManager>();

        if (_player == null || _player.SkillCollection == null)
        {
            Debug.LogError("SkillSlotView 找不到玩家技能系统。", this);
            SetEmptyState();
            enabled = false;
            return;
        }

        _player.SkillBarChanged += OnSkillBarChanged;
        RefreshBinding();
    }

    private void OnDestroy()
    {
        ReleaseInput();
        UnbindRuntime();

        if (_player != null)
            _player.SkillBarChanged -= OnSkillBarChanged;
    }

    internal void PressInput()
    {
        if (_entry == null || _entry.Definition == null ||
            _entry.Definition.ActivationType != SkillActivationType.Active)
        {
            return;
        }

        _player.PressSkillSlot(slot);
    }

    internal void ReleaseInput()
    {
        _player?.ReleaseSkillSlot(slot);
    }

    private void RefreshBinding()
    {
        UnbindRuntime();

        if (hotkeyText != null)
        {
            hotkeyText.text = _player != null && _player.Input != null
                ? _player.Input.GetBindingDisplayString(slot)
                : string.Empty;
        }

        _entry = _player != null
            ? _player.GetSkillEntry(slot)
            : null;

        if (backgroundImage != null)
            backgroundImage.enabled = true;

        if (_entry == null || _entry.Definition == null)
        {
            SetEmptyState();
            return;
        }

        if (iconImage != null)
        {
            iconImage.sprite = _entry.Definition.Icon;
            iconImage.enabled = iconImage.sprite != null;
        }

        if (skillButton != null)
        {
            skillButton.interactable =
                _entry.Definition.ActivationType == SkillActivationType.Active;
        }

        _runtime = _entry.Runtime;
        _runtime.CooldownChanged += SetCooldown;
        SetCooldown(
            _runtime.RemainingCooldown,
            _entry.Definition.Cooldown);
    }

    private void SetEmptyState()
    {
        _entry = null;

        if (backgroundImage != null)
            backgroundImage.enabled = true;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (skillButton != null)
            skillButton.interactable = false;

        SetCooldown(0f, 0f);
    }

    private void SetCooldown(float remaining, float total)
    {
        if (cooldownFill == null)
            return;

        bool isCoolingDown = remaining > 0f && total > 0f;
        cooldownFill.fillAmount = isCoolingDown
            ? Mathf.Clamp01(remaining / total)
            : 0f;
        cooldownFill.enabled = isCoolingDown;
    }

    private void UnbindRuntime()
    {
        if (_runtime != null)
            _runtime.CooldownChanged -= SetCooldown;

        _runtime = null;
    }

    private void OnSkillBarChanged(
        int changedSlot,
        SkillDefinition definition)
    {
        if (changedSlot == (int)slot)
            RefreshBinding();
    }
}
