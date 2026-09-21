using UnityEngine;

[CreateAssetMenu(
    fileName = "PeriodicDamageBuffEffect",
    menuName = "ARPG/Buffs/Effects/Periodic Damage")]
public sealed class PeriodicDamageBuffEffect : BuffLifecycleEffect
{
    [SerializeField, Min(0)] private int damagePerStack = 1;

    public override void OnTick(BuffContext context)
    {
        IDamageable damageable = context.Target as IDamageable;
        if (damageable == null)
            return;

        int damage = Mathf.Max(
            0,
            Mathf.RoundToInt(
                damagePerStack * context.StackCount * context.Potency));

        damageable.TakeDamage(damage);
    }
}
