using UnityEngine;

[ExecuteAlways]
public sealed class StartMenuDesignFit : MonoBehaviour
{
    private void OnEnable() => Fit();
    private void OnRectTransformDimensionsChange() => Fit();

    private void Fit()
    {
        RectTransform parent = transform.parent as RectTransform;
        if (parent == null) return;
        float scale = Mathf.Min(parent.rect.width / 1920f, parent.rect.height / 1080f);
        transform.localScale = Vector3.one * scale;
    }
}
