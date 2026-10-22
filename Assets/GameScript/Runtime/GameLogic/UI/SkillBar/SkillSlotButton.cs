using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public sealed class SkillSlotButton : MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    [SerializeField] private SkillSlotView view;

    private Button _button;
    private bool _isPointerHeld;

    private void Awake()
    {
        _button = GetComponent<Button>();

        if (view == null)
            view = GetComponentInParent<SkillSlotView>();
    }

    public void Initialize(SkillSlotView slotView)
    {
        view = slotView;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left ||
            _button == null || !_button.interactable)
        {
            return;
        }

        _isPointerHeld = true;
        view?.PressInput();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            ReleasePointer();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ReleasePointer();
    }

    private void OnDisable()
    {
        ReleasePointer();
    }

    private void ReleasePointer()
    {
        if (!_isPointerHeld)
            return;

        _isPointerHeld = false;
        view?.ReleaseInput();
    }
}
