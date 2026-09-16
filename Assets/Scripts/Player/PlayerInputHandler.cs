using System;
using UnityEngine;

public sealed class PlayerInputHandler : IDisposable
{
    private readonly NewActions _actions = new NewActions();

    public Vector2 MoveInput => _actions.player.Move.ReadValue<Vector2>();
    public bool AttackPressedThisFrame => _actions.player.Attack.WasPressedThisFrame();

    public void Enable()
    {
        _actions.player.Enable();
    }

    public void Disable()
    {
        _actions.player.Disable();
    }

    public void Dispose()
    {
        _actions.Dispose();
    }
}
