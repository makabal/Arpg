using UnityEngine;
using UnityEngine.InputSystem;

public sealed class TabPageToggleController : MonoBehaviour
{
    [SerializeField] private GameObject tabPage;
    [SerializeField] private TopBarTabGroup tabGroup;

    private NewActions actions;

    private void Awake()
    {
        actions = new NewActions();
        SetVisible(false);
    }

    private void OnEnable()
    {
        actions.player.Tab.performed += OnTabPerformed;
        actions.player.Tab.Enable();
    }

    private void OnDisable()
    {
        if (actions == null)
            return;

        actions.player.Tab.performed -= OnTabPerformed;
        actions.player.Tab.Disable();
    }

    private void OnDestroy()
    {
        actions?.Dispose();
    }

    private void OnTabPerformed(InputAction.CallbackContext context)
    {
        if (tabPage == null)
            return;

        SetVisible(!tabPage.activeSelf);
    }

    private void SetVisible(bool visible)
    {
        if (tabPage == null)
            return;

        tabPage.SetActive(visible);

        if (visible && tabGroup != null)
            tabGroup.Select(TopBarPage.Character);
    }
}
