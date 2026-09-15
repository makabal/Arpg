using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    private static readonly int HitHash = Animator.StringToHash("Hit");

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
