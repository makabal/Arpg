using UnityEngine;

public readonly struct SkillTarget
{
    public MonoBehaviour Owner { get; }
    public Transform Transform => Owner != null ? Owner.transform : null;
    public IDamageable Damageable { get; }
    public Health Health => Damageable?.Health;

    public SkillTarget(MonoBehaviour owner, IDamageable damageable)
    {
        Owner = owner;
        Damageable = damageable;
    }

    public static bool TryCreate(
        Component component,
        out SkillTarget skillTarget)
    {
        if (component != null)
        {
            MonoBehaviour[] behaviours =
                component.GetComponentsInParent<MonoBehaviour>();

            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour is IDamageable damageable)
                {
                    skillTarget = new SkillTarget(
                        behaviour,
                        damageable);

                    return true;
                }
            }
        }

        skillTarget = default;
        return false;
    }
}
