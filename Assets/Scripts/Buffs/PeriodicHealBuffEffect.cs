using UnityEngine;

[CreateAssetMenu(
    fileName = "PeriodicHealBuffEffect",
    menuName = "ARPG/Buffs/Effects/Periodic Heal")]
public sealed class PeriodicHealBuffEffect : BuffLifecycleEffect
{
    [SerializeField, Min(0)] private int healingPerStack = 1;

    public override void OnTick(BuffContext context)
    {
        IDamageable damageable = context.Target as IDamageable;
        if (damageable == null || damageable.Health == null)
            return;

        int amount = Mathf.Max(
            0,
            Mathf.RoundToInt(
                healingPerStack * context.StackCount * context.Potency));

        damageable.Health.Restore(amount);
    }
}
