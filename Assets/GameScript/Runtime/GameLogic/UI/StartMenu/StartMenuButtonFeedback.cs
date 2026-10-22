using UnityEngine;
using UnityEngine.EventSystems;

public sealed class StartMenuButtonFeedback : MonoBehaviour, IPointerEnterHandler,
    IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField] private GameObject selectionArrow;
    private RectTransform rect;
    private UnityEngine.UI.Image image;
    private Vector2 restPosition;
    private bool hovered;
    private bool selected;
    private bool pressed;

    public void Configure(GameObject arrow) => selectionArrow = arrow;

    private void Awake()
    {
        rect = (RectTransform)transform;
        image = GetComponent<UnityEngine.UI.Image>();
        restPosition = rect.anchoredPosition;
    }

    private void Refresh()
    {
        bool highlighted = hovered || selected;
        image.color = highlighted ? new Color(1f, 1f, 0.94f) : new Color(0.86f, 0.9f, 0.83f);
        rect.localScale = Vector3.one * (highlighted ? (selectionArrow != null ? 1.035f : 1.025f) : 1f);
        rect.anchoredPosition = restPosition + (pressed ? Vector2.down * 3f : Vector2.zero);
        if (selectionArrow != null) selectionArrow.SetActive(highlighted);
    }

    public void OnPointerEnter(PointerEventData data) { hovered = true; Refresh(); }
    public void OnPointerExit(PointerEventData data) { hovered = false; pressed = false; Refresh(); }
    public void OnPointerDown(PointerEventData data) { if (data.button == PointerEventData.InputButton.Left) { pressed = true; Refresh(); } }
    public void OnPointerUp(PointerEventData data) { pressed = false; Refresh(); }
    public void OnSelect(BaseEventData data) { selected = true; Refresh(); }
    public void OnDeselect(BaseEventData data) { selected = false; Refresh(); }

    private void OnDisable()
    {
        hovered = selected = pressed = false;
        if (rect != null) Refresh();
    }
}
