using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class EnemyTargetSelector : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private Camera worldCamera;
    [SerializeField] private EnemyTargetHud targetHud;
    [SerializeField] private Material selectedMaterial;

    [Header("检测")]
    [SerializeField] private LayerMask selectableLayers = ~0;

    private EnemyManager _selectedEnemy;
    private Health _selectedHealth;
    private SpriteRenderer _selectedRenderer;
    private Material _originalMaterial;

    private void Awake()
    {
        if (worldCamera == null)
            worldCamera = Camera.main;

        targetHud.Hide();
    }

    private void Update()
    {
        if (Mouse.current == null ||
            !Mouse.current.leftButton.wasPressedThisFrame)
        {
            return;
        }

        // 点击 UI 时不选择场景中的敌人。
        if (EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        Vector2 screenPosition = Mouse.current.position.ReadValue();
        Vector2 worldPosition =
            worldCamera.ScreenToWorldPoint(screenPosition);

        SelectAt(worldPosition);
    }

    private void SelectAt(Vector2 worldPosition)
    {
        EnemyManager enemy = null;

        Collider2D[] hits =
            Physics2D.OverlapPointAll(worldPosition, selectableLayers);

        foreach (Collider2D hit in hits)
        {
            enemy = hit.GetComponentInParent<EnemyManager>();

            if (enemy != null)
                break;
        }

        SetSelection(enemy);
    }

    private void SetSelection(EnemyManager enemy)
    {
        if (_selectedEnemy == enemy)
            return;

        ClearSelection();

        if (enemy == null)
            return;

        _selectedEnemy = enemy;
        _selectedHealth = enemy.Health;
        _selectedRenderer =
            enemy.GetComponentInChildren<SpriteRenderer>();

        if (_selectedRenderer != null)
        {
            _originalMaterial = _selectedRenderer.sharedMaterial;
            _selectedRenderer.sharedMaterial = selectedMaterial;
        }

        _selectedHealth.Died += OnSelectedEnemyDied;
        targetHud.Show(enemy);
    }

    private void ClearSelection()
    {
        if (_selectedHealth != null)
            _selectedHealth.Died -= OnSelectedEnemyDied;

        if (_selectedRenderer != null)
            _selectedRenderer.sharedMaterial = _originalMaterial;

        _selectedEnemy = null;
        _selectedHealth = null;
        _selectedRenderer = null;
        _originalMaterial = null;

        targetHud.Hide();
    }

    private void OnSelectedEnemyDied()
    {
        ClearSelection();
    }

    private void OnDisable()
    {
        ClearSelection();
    }
}