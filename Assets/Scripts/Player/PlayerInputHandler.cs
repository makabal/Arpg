using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerInputHandler : IDisposable
{
    private readonly NewActions _actions = new NewActions();
    private readonly InputAction[] _skillActions =
        new InputAction[PlayerManager.SkillBarSlotCount];
    private readonly bool[] _keyboardHeldSkillSlots =
        new bool[PlayerManager.SkillBarSlotCount];
    private readonly bool[] _uiHeldSkillSlots =
        new bool[PlayerManager.SkillBarSlotCount];

    public PlayerInputHandler()
    {
        _skillActions[(int)SkillSlot.BasicAttack] = _actions.player.Attack;
        _skillActions[(int)SkillSlot.Skill1] = _actions.player.Skill1;
        _skillActions[(int)SkillSlot.Skill2] = _actions.player.Skill2;
        _skillActions[(int)SkillSlot.Skill3] = _actions.player.Skill3;
        _skillActions[(int)SkillSlot.Skill4] = _actions.player.Skill4;
        _skillActions[(int)SkillSlot.Skill5] = _actions.player.Skill5;

        for (int i = 0; i < _skillActions.Length; i++)
            TrackHeldState(_skillActions[i], i);
    }

    public Vector2 MoveInput => _actions.player.Move.ReadValue<Vector2>();

    public bool TryGetPressedSkillSlot(out SkillSlot slot)
    {
        for (int i = 0; i < _skillActions.Length; i++)
        {
            if (_skillActions[i].WasPressedThisFrame())
            {
                slot = (SkillSlot)i;
                return true;
            }
        }

        slot = default;
        return false;
    }

    public string GetBindingDisplayString(SkillSlot slot)
    {
        int slotIndex = (int)slot;
        return slotIndex >= 0 && slotIndex < _skillActions.Length
            ? _skillActions[slotIndex].GetBindingDisplayString()
            : string.Empty;
    }

    public bool IsSkillSlotHeld(int slot)
    {
        return slot >= 0 &&
            slot < _keyboardHeldSkillSlots.Length &&
            (_keyboardHeldSkillSlots[slot] ||
                _uiHeldSkillSlots[slot]);
    }

    public void SetUISkillSlotHeld(
        SkillSlot slot,
        bool isHeld)
    {
        int slotIndex = (int)slot;

        if (slotIndex < 0 || slotIndex >= _uiHeldSkillSlots.Length)
            return;

        _uiHeldSkillSlots[slotIndex] = isHeld;
    }

    public void Enable()
    {
        _actions.player.Enable();
    }

    public void Disable()
    {
        _actions.player.Disable();
        Array.Clear(
            _keyboardHeldSkillSlots,
            0,
            _keyboardHeldSkillSlots.Length);
        Array.Clear(
            _uiHeldSkillSlots,
            0,
            _uiHeldSkillSlots.Length);
    }

    public void Dispose()
    {
        _actions.Dispose();
    }

    private void TrackHeldState(
        InputAction action,
        int slotIndex)
    {
        action.started += _ =>
            _keyboardHeldSkillSlots[slotIndex] = true;
        action.canceled += _ =>
            _keyboardHeldSkillSlots[slotIndex] = false;
    }
}
