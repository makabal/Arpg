using UnityEngine;

public abstract class SkillAimResolver : ScriptableObject
{
    public abstract SkillUseFailure Resolve(
        in SkillAimRequest request,
        out SkillAimData aim);
}
