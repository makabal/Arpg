using UnityEngine;

[CreateAssetMenu(
    fileName = "ProjectileDelivery",
    menuName = "ARPG/Skills/Delivery/Projectile")]
public sealed class ProjectileDelivery : SkillDelivery
{
    [SerializeField] private SkillProjectile projectilePrefab;
    [SerializeField] private bool spawnAtAttackPoint = true;
    [SerializeField] private LayerMask targetLayers;
    [SerializeField, Min(0.01f)] private float speed = 8f;
    [SerializeField, Min(0.01f)] private float lifetime = 5f;
    [SerializeField, Min(1)] private int maxHits = 1;
    [SerializeField] private bool destroyWhenMaxHitsReached = true;

    public override ISkillDeliveryHandle Deliver(
        SkillCastContext context)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning(
                $"技能 {context.Definition.DisplayName} 没有配置投射物预制体。",
                context.Caster);
            return null;
        }

        Transform attackPoint = context.Caster.AttackPoint;
        Vector2 spawnPosition = spawnAtAttackPoint && attackPoint != null
            ? attackPoint.position
            : context.Caster.transform.position;

        float angle = Mathf.Atan2(
            context.Aim.Direction.y,
            context.Aim.Direction.x) * Mathf.Rad2Deg;

        SkillProjectile projectile = Object.Instantiate(
            projectilePrefab,
            spawnPosition,
            Quaternion.Euler(0f, 0f, angle));

        projectile.Initialize(
            context,
            targetLayers,
            speed,
            lifetime,
            maxHits,
            destroyWhenMaxHitsReached);

        // 投射物发射后独立存在，不随施法状态结束而销毁。
        return null;
    }
}
