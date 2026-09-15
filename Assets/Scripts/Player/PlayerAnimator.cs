using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    
    private PlayerInputHandler _input;
    private Animator _animator;

    void Awake()
    {
        _input    = GetComponent<PlayerInputHandler>();
        _animator = GetComponentInChildren<Animator>();
    }
    
    void Update()
    {
        _animator.SetFloat(SpeedHash, _input.MoveInput.sqrMagnitude);
    }

    public void PlayAttackAnimation()
    {
        _animator.SetTrigger(AttackHash);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

}
