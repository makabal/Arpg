using UnityEngine;

public sealed class EnemyAnimationController
{
    private static readonly int HitHash = Animator.StringToHash("Hit");

    private readonly Animator _animator;

    public EnemyAnimationController(Animator animator)
    {
        _animator = animator;
    }

    public void PlayHitAnimation()
    {
        _animator.SetBool(HitHash, true);
    }

    public void EndHitAnimation()
    {
        _animator.SetBool(HitHash, false);
    }
}
