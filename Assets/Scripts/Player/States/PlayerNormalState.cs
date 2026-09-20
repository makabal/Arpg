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

        if (Owner.Input.TryGetPressedSkillSlot(out SkillSlot slot))
            Owner.TryUseSkillSlot(slot);
    }

    public override void FixedUpdate()
    {
        Owner.ApplyMovement();
    }
}
