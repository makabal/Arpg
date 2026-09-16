using UnityEngine;

[DisallowMultipleComponent]
// Sorts a 2D character from the world-space Y position of its feet.
public sealed class YSortRenderer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Transform sortPoint;

    [SerializeField, Min(1)]
    private int precision = 100;

    [SerializeField]
    private int orderOffset;

    private void Reset()
    {
        targetRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<SpriteRenderer>();

        if (sortPoint == null)
            sortPoint = transform;
    }

    private void LateUpdate()
    {
        if (targetRenderer == null || sortPoint == null)
            return;

        // Y 越小，Order 越大，因此显示在前面。
        targetRenderer.sortingOrder =
            orderOffset -
            Mathf.RoundToInt(sortPoint.position.y * precision);
    }
}
