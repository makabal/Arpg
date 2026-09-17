public sealed class PlayerNormalState : PlayerState
{
    public PlayerNormalState(
        PlayerManager player,
        StateMachine<PlayerManager> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Update()
    {
        Owner.ReadMovementInput();

        if (Owner.Input.TryGetPressedSkillSlot(out int slot) &&
            Owner.Skills.TryBeginUse(slot) == SkillUseFailure.None)
        {
            StateMachine.ChangeState(Owner.SkillState);
        }
    }

    public override void FixedUpdate()
    {
        Owner.ApplyMovement();
    }
}
