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
}
