using System;
using UnityEngine;

[Serializable]
public sealed class SkillCastSettings
{
    [SerializeField] private SkillReleaseMode releaseMode;
    [SerializeField] private SkillDurationMode durationMode;
    [SerializeField] private SkillCooldownStartMode cooldownStartMode;
    [SerializeField, Min(0f)] private float effectTime;
    [Tooltip("持续引导技能设为 0 时不限制最大持续时间。")]
    [SerializeField, Min(0f)] private float totalDuration;
    [SerializeField] private bool canMoveWhileCasting;
    [SerializeField] private bool canBeInterrupted = true;

    public SkillReleaseMode ReleaseMode => releaseMode;
    public SkillDurationMode DurationMode => durationMode;
    public SkillCooldownStartMode CooldownStartMode => cooldownStartMode;
    public float EffectTime => Mathf.Max(0f, effectTime);
    public float TotalDuration
    {
        get
        {
            if (durationMode == SkillDurationMode.WhileInputHeld &&
                totalDuration <= 0f)
            {
                return 0f;
            }

            return Mathf.Max(EffectTime, totalDuration);
        }
    }
    public bool CanMoveWhileCasting => canMoveWhileCasting;
    public bool CanBeInterrupted => canBeInterrupted;
}
