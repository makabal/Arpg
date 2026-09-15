using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private NewActions _actions;
    
    public Vector2 MoveInput=> _actions.player.Move.ReadValue<Vector2>();
    public bool AttackPressedThisFrame => _actions.player.Attack.WasPressedThisFrame();
    
    void Awake()
    {
        _actions = new NewActions();
    }

    void OnEnable()
    {
        _actions.player.Enable();
    }

    
    void OnDisable()
    {
        _actions.player.Disable();
    }

    void OnDestroy()
    {
        _actions.Dispose();
    }

}