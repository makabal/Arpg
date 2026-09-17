using UnityEngine;

public abstract class SkillCondition : ScriptableObject
{
    public abstract SkillUseFailure Validate(SkillCastContext context);
}
