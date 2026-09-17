using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerInputHandler : IDisposable
{
    private readonly NewActions _actions = new NewActions();
    private readonly bool[] _heldSkillSlots =
        new bool[(int)SkillSlot.Skill5 + 1];

    public PlayerInputHandler()
    {
        TrackHeldState(
            _actions.player.Attack,
            SkillSlot.BasicAttack);
        TrackHeldState(
            _actions.player.Skill1,
            SkillSlot.Skill1);
        TrackHeldState(
            _actions.player.Skill2,
            SkillSlot.Skill2);
        TrackHeldState(
            _actions.player.Skill3,
            SkillSlot.Skill3);
        TrackHeldState(
            _actions.player.Skill4,
            SkillSlot.Skill4);
        TrackHeldState(
            _actions.player.Skill5,
            SkillSlot.Skill5);
    }

    public Vector2 MoveInput => _actions.player.Move.ReadValue<Vector2>();

    public bool TryGetPressedSkillSlot(out int slot)
    {
        if (_actions.player.Attack.WasPressedThisFrame())
            slot = (int)SkillSlot.BasicAttack;
        else if (_actions.player.Skill1.WasPressedThisFrame())
            slot = (int)SkillSlot.Skill1;
        else if (_actions.player.Skill2.WasPressedThisFrame())
            slot = (int)SkillSlot.Skill2;
        else if (_actions.player.Skill3.WasPressedThisFrame())
            slot = (int)SkillSlot.Skill3;
        else if (_actions.player.Skill4.WasPressedThisFrame())
            slot = (int)SkillSlot.Skill4;
        else if (_actions.player.Skill5.WasPressedThisFrame())
            slot = (int)SkillSlot.Skill5;
        else
        {
            slot = -1;
            return false;
        }

        _heldSkillSlots[slot] = true;
        return true;
    }

    public bool IsSkillSlotHeld(int slot)
    {
        return slot >= 0 &&
            slot < _heldSkillSlots.Length &&
            _heldSkillSlots[slot];
    }

    public void Enable()
    {
        _actions.player.Enable();
    }

    public void Disable()
    {
        _actions.player.Disable();
        Array.Clear(
            _heldSkillSlots,
            0,
            _heldSkillSlots.Length);
    }

    public void Dispose()
    {
        _actions.Dispose();
    }

    private void TrackHeldState(
        InputAction action,
        SkillSlot slot)
    {
        int slotIndex = (int)slot;
        action.started += _ =>
            _heldSkillSlots[slotIndex] = true;
        action.canceled += _ =>
            _heldSkillSlots[slotIndex] = false;
    }
}
