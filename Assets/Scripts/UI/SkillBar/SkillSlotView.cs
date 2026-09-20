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
    [SerializeField] private Image lockImage;
    [SerializeField] private TMP_Text hotkeyText;
    [SerializeField] private Button skillButton;

    private PlayerManager _player;
    private PlayerSkillCollection _skillCollection;
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

        _skillCollection = _player.SkillCollection;
        _player.SkillBarChanged += OnSkillBarChanged;
        _skillCollection.UnlockChanged += OnUnlockChanged;
        RefreshBinding();
    }

    private void OnDestroy()
    {
        ReleaseInput();
        UnbindRuntime();

        if (_player != null)
            _player.SkillBarChanged -= OnSkillBarChanged;

        if (_skillCollection != null)
            _skillCollection.UnlockChanged -= OnUnlockChanged;
    }

    internal void PressInput()
    {
        if (_entry == null || !_entry.IsUnlocked ||
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

        bool unlocked = _entry.IsUnlocked;

        if (iconImage != null)
        {
            iconImage.sprite = unlocked
                ? _entry.Definition.Icon
                : null;
            iconImage.enabled = unlocked && iconImage.sprite != null;
        }

        if (lockImage != null)
            lockImage.gameObject.SetActive(!unlocked);

        if (skillButton != null)
        {
            skillButton.interactable = unlocked &&
                _entry.Definition.ActivationType ==
                    SkillActivationType.Active;
        }

        if (!unlocked)
        {
            SetCooldown(0f, 0f);
            return;
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

        if (lockImage != null)
            lockImage.gameObject.SetActive(false);

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

    private void OnUnlockChanged(PlayerSkillEntry entry)
    {
        if (ReferenceEquals(entry, _entry))
            RefreshBinding();
    }
}
