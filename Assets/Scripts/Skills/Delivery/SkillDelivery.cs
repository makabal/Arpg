using UnityEngine;

public abstract class SkillDelivery : ScriptableObject
{
    public abstract ISkillDeliveryHandle Deliver(
        SkillCastContext context);
}
