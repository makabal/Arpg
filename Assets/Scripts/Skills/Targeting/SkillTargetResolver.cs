using System.Collections.Generic;
using UnityEngine;

public abstract class SkillTargetResolver : ScriptableObject
{
    public abstract SkillTargetType TargetType { get; }

    public abstract SkillUseFailure Resolve(
        in SkillTargetRequest request,
        List<SkillTarget> results);
}
