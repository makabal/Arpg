using UnityEngine;

public sealed class PlayerSkillState : PlayerState
{
    private float _elapsed;
    private SkillPresentationData _activePresentation;

    public PlayerSkillState(
        PlayerManager player,
        StateMachine<PlayerManager> stateMachine)
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        _elapsed = 0f;

        _activePresentation =
            Owner.Skills.ActiveDefinition?.Presentation;

        Owner.Animation.PlaySkillAnimation(_activePresentation);

        if (!CanMoveWhileCasting())
            Owner.StopMovement();
    }

    public override void Update()
    {
        SkillDefinition definition = Owner.Skills.ActiveDefinition;

        if (definition == null)
        {
            StateMachine.ChangeState(Owner.NormalState);
            return;
        }

        if (CanMoveWhileCasting())
            Owner.ReadMovementInput();

        _elapsed += Time.deltaTime;

        bool usesAnimationEvents = UsesAnimationEvents(definition);

        if (!usesAnimationEvents &&
            !Owner.Skills.ActiveSkillReleased &&
            _elapsed >= definition.CastSettings.EffectTime)
        {
            Owner.Skills.ReleaseActiveSkill();
        }

        if (definition.CastSettings.DurationMode ==
            SkillDurationMode.WhileInputHeld)
        {
            bool isHeld = Owner.Input.IsSkillSlotHeld(
                Owner.Skills.ActiveSlot);
            bool hasEnoughMana = !isHeld ||
                Owner.Skills.TryMaintainChannel(Time.deltaTime);
            bool reachedMaximumDuration =
                definition.CastSettings.TotalDuration > 0f &&
                _elapsed >= definition.CastSettings.TotalDuration;

            if (!isHeld || !hasEnoughMana || reachedMaximumDuration)
                FinishSkill();

            return;
        }

        if (usesAnimationEvents)
            return;

        if (_elapsed < definition.CastSettings.TotalDuration)
            return;

        Owner.Skills.ReleaseActiveSkill();
        Owner.Skills.FinishActiveSkill();
        StateMachine.ChangeState(Owner.NormalState);
    }

    public override void FixedUpdate()
    {
        if (CanMoveWhileCasting())
            Owner.ApplyMovement();
        else
            Owner.StopMovement();
    }

    public override void Exit()
    {
        Owner.Animation.StopSkillAnimation(_activePresentation);
        _activePresentation = null;
        Owner.Skills.CancelActiveSkill(true);
    }

    public void ReleaseSkill()
    {
        Owner.Skills.ReleaseActiveSkill();
    }

    public void FinishSkill()
    {
        if (!Owner.Skills.HasActiveSkill)
            return;

        Owner.Skills.FinishActiveSkill();
        StateMachine.ChangeState(Owner.NormalState);
    }

    private bool CanMoveWhileCasting()
    {
        return Owner.Skills.ActiveDefinition != null &&
            Owner.Skills.ActiveDefinition.CastSettings
                .CanMoveWhileCasting;
    }

    private static bool UsesAnimationEvents(
        SkillDefinition definition)
    {
        return definition.CastSettings.ReleaseMode ==
                SkillReleaseMode.AnimationEvents &&
            definition.Presentation != null &&
            !string.IsNullOrWhiteSpace(
                definition.Presentation.AnimationParameter);
    }
}
