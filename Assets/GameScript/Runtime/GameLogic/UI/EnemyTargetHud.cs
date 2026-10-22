using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class EnemyTargetHud : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private TMP_Text enemyNameText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;

    private EnemyManager _target;
    private Health _health;

    public void Show(EnemyManager target)
    {
        if (target == null)
        {
            Hide();
            return;
        }

        UnbindTarget();

        _target = target;
        _health = target.Health;

        _health.Changed += RefreshHealth;
        _health.Died += OnTargetDied;

        enemyNameText.text = target.DisplayName;

        RefreshHealth(
            _health.CurrentHealth,
            _health.MaxHealth);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        UnbindTarget();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        UnbindTarget();
    }

    private void UnbindTarget()
    {
        if (_health != null)
        {
            _health.Changed -= RefreshHealth;
            _health.Died -= OnTargetDied;
        }

        _target = null;
        _health = null;
    }

    private void RefreshHealth(int current, int max)
    {
        if (_health.IsInfinite)
        {
            healthSlider.normalizedValue = 1f;
            healthText.text = "∞";
            return;
        }

        healthSlider.normalizedValue = max > 0
            ? Mathf.Clamp01((float)current / max)
            : 0f;

        healthText.text = $"{current} / {max}";
    }

    private void OnTargetDied()
    {
        Hide();
    }
}