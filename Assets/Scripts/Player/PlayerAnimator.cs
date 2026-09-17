using UnityEngine;

public sealed class PlayerAnimator
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    private readonly Animator _animator;

    public PlayerAnimator(Animator animator)
    {
        _animator = animator;
    }

    public void SetMoveSpeed(Vector2 moveInput)
    {
        _animator.SetFloat(SpeedHash, moveInput.sqrMagnitude);
    }

    public void PlayAttackAnimation()
    {
        _animator.SetTrigger(AttackHash);
    }

    public void PlaySkillAnimation(SkillPresentationData presentation)
    {
        if (presentation == null)
            return;

        if (presentation.AnimationParameterType ==
                SkillAnimationParameterType.Bool &&
            !string.IsNullOrWhiteSpace(
                presentation.AnimationParameter))
        {
            _animator.SetBool(presentation.AnimationParameter, true);
        }

        if (!string.IsNullOrWhiteSpace(
                presentation.AnimationStartTrigger))
        {
            _animator.SetTrigger(
                presentation.AnimationStartTrigger);
        }
        else if (presentation.AnimationParameterType ==
                     SkillAnimationParameterType.Trigger &&
                 !string.IsNullOrWhiteSpace(
                     presentation.AnimationParameter))
        {
            _animator.SetTrigger(presentation.AnimationParameter);
        }
    }

    public void StopSkillAnimation(SkillPresentationData presentation)
    {
        if (presentation == null)
            return;

        if (!string.IsNullOrWhiteSpace(
                presentation.AnimationStartTrigger))
        {
            _animator.ResetTrigger(
                presentation.AnimationStartTrigger);
        }

        if (presentation.AnimationParameterType ==
                SkillAnimationParameterType.Bool &&
            !string.IsNullOrWhiteSpace(
                presentation.AnimationParameter))
        {
            _animator.SetBool(presentation.AnimationParameter, false);
        }
    }
}
