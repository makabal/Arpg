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
        if (Owner.Buffs != null && Owner.Buffs.HasTag(BuffTag.Stun))
        {
            Owner.StopMovement();
            return;
        }

        Owner.ReadMovementInput();

        if (Owner.Input.TryGetPressedSkillSlot(out SkillSlot slot))
            Owner.TryUseSkillSlot(slot);
    }

    public override void FixedUpdate()
    {
        if (Owner.Buffs != null && Owner.Buffs.HasTag(BuffTag.Stun))
            Owner.StopMovement();
        else
            Owner.ApplyMovement();
    }
}
