using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("攻击范围")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.8f;
    [SerializeField] private LayerMask enemyLayer;
    
    private PlayerInputHandler _input;
    private PlayerAnimator _playerAnimator;
    private bool _isAttacking;
    
    private void Awake()
    {
        _input = GetComponent<PlayerInputHandler>();
        _playerAnimator = GetComponent<PlayerAnimator>();
    }
    
    private void Update()
    {
        if (_input.AttackPressedThisFrame && !_isAttacking)
        {
            StartAttack();
        }
    }
    
    private void StartAttack()
    {
        _isAttacking = true;
        _playerAnimator.PlayAttackAnimation();
    }
    
    public void AttackHit()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer);

        foreach (Collider2D target in targets)
        {
            float targetX = target.bounds.center.x;
            float playerX = transform.position.x;

            bool facingRight = transform.localScale.x > 0;
            bool targetInFront = facingRight
                ? targetX > playerX
                : targetX < playerX;

            if (!targetInFront)
                continue;

            EnemyAnimationController enemyAnimator =
                target.GetComponentInParent<EnemyAnimationController>();

            if (enemyAnimator != null)
            {
                enemyAnimator.PlayHitAnimation();
            }
        }
    }
    
    public void EndAttack()
    {
        _isAttacking = false;
    }
    
}
