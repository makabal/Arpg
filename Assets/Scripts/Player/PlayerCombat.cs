using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerCombat
{
    private readonly Transform _owner;
    private readonly Transform _attackPoint;
    private readonly float _attackRadius;
    private readonly LayerMask _enemyLayer;
    private readonly int _attackDamage;

    public PlayerCombat(
        Transform owner,
        Transform attackPoint,
        float attackRadius,
        LayerMask enemyLayer,
        int attackDamage)
    {
        _owner = owner;
        _attackPoint = attackPoint;
        _attackRadius = attackRadius;
        _enemyLayer = enemyLayer;
        _attackDamage = attackDamage;
    }

    public void AttackHit()
    {
        if (_attackPoint == null)
            return;

        Collider2D[] targets = Physics2D.OverlapCircleAll(
            _attackPoint.position,
            _attackRadius,
            _enemyLayer);

        var damagedTargets = new HashSet<IDamageable>();

        foreach (Collider2D target in targets)
        {
            float targetX = target.bounds.center.x;
            float attackPointX = _attackPoint.position.x;
            bool facingRight = _owner.localScale.x > 0f;
            bool attackPointHasNotPassedTarget = facingRight
                ? attackPointX <= targetX
                : attackPointX >= targetX;

            if (!attackPointHasNotPassedTarget)
                continue;

            MonoBehaviour[] behaviours = target.GetComponentsInParent<MonoBehaviour>();
            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IDamageable damageable && damagedTargets.Add(damageable))
                {
                    damageable.TakeDamage(_attackDamage);
                    break;
                }
            }
        }
    }
}
