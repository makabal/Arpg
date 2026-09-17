using System;
using UnityEngine;

public abstract class SkillPassiveTrigger : ScriptableObject
{
    public abstract IDisposable Bind(PassiveSkillRuntime runtime);
}
