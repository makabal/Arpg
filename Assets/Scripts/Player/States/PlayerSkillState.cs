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
            Owner.Skills.ActiveBuild?.Presentation;

        Owner.Animation.PlaySkillAnimation(_activePresentation);

        if (!CanMoveWhileCasting())
            Owner.StopMovement();
    }

    public override void Update()
    {
        if (Owner.Buffs != null && Owner.Buffs.HasTag(BuffTag.Stun))
        {
            StateMachine.ChangeState(Owner.NormalState);
            return;
        }

        SkillDefinition definition = Owner.Skills.ActiveDefinition;

        if (definition == null)
        {
            StateMachine.ChangeState(Owner.NormalState);
            return;
        }

        if (CanMoveWhileCasting())
            Owner.ReadMovementInput();

        _elapsed += Time.deltaTime;

        bool usesAnimationEvents = UsesAnimationEvents(
            definition,
            Owner.Skills.ActiveBuild?.Presentation);
        float effectTime = Owner.Skills.ActiveBuild.GetNumeric(
            SkillNumericStat.CastEffectTime,
            definition.CastSettings.EffectTime);
        float totalDuration = Owner.Skills.ActiveBuild.GetNumeric(
            SkillNumericStat.CastTotalDuration,
            definition.CastSettings.TotalDuration);

        if (!usesAnimationEvents &&
            !Owner.Skills.ActiveSkillReleased &&
            _elapsed >= effectTime)
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
                totalDuration > 0f &&
                _elapsed >= totalDuration;

            if (!isHeld || !hasEnoughMana || reachedMaximumDuration)
                FinishSkill();

            return;
        }

        if (usesAnimationEvents)
            return;

        if (_elapsed < totalDuration)
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
        SkillDefinition definition,
        SkillPresentationData presentation)
    {
        return definition.CastSettings.ReleaseMode ==
                SkillReleaseMode.AnimationEvents &&
            presentation != null &&
            !string.IsNullOrWhiteSpace(
                presentation.AnimationParameter);
    }
}
