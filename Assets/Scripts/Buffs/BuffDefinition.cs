using System;
using UnityEngine;

[Flags]
public enum BuffTag
{
    None = 0,
    Buff = 1 << 0,
    Debuff = 1 << 1,
    DamageOverTime = 1 << 2,
    CrowdControl = 1 << 3,
    Stun = 1 << 4,
    Silence = 1 << 5,
    Invulnerable = 1 << 6,
    Dispellable = 1 << 7
}

public enum BuffStackMode
{
    Ignore,
    RefreshDuration,
    AddStack,
    Independent
}

public enum BuffDurationMode
{
    Fixed,
    Permanent
}

[Serializable]
public sealed class BuffStatModifier
{
    [SerializeField] private CharacterStat stat;
    [SerializeField] private StatModifierOperation operation;
    [SerializeField] private float value;
    [SerializeField] private bool scaleWithStacks = true;

    public CharacterStat Stat => stat;
    public StatModifierOperation Operation => operation;
    public float Value => value;
    public bool ScaleWithStacks => scaleWithStacks;
}

[CreateAssetMenu(
    fileName = "Buff",
    menuName = "ARPG/Buffs/Buff Definition")]
public sealed class BuffDefinition : ScriptableObject
{
    [Header("基本信息")]
    [SerializeField] private string buffId;
    [SerializeField] private string displayName;
    [SerializeField] private Sprite icon;
    [SerializeField] private BuffTag tags = BuffTag.Buff;

    [Header("持续与叠层")]
    [SerializeField] private BuffDurationMode durationMode;
    [SerializeField, Min(0f)] private float duration = 5f;
    [SerializeField, Min(0f)] private float tickInterval;
    [SerializeField, Min(1)] private int maxStacks = 1;
    [SerializeField] private BuffStackMode stackMode =
        BuffStackMode.RefreshDuration;

    [Header("效果")]
    [SerializeField] private BuffStatModifier[] statModifiers;
    [SerializeField] private BuffLifecycleEffect[] lifecycleEffects;

    public string BuffId => buffId;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public BuffTag Tags => tags;
    public BuffDurationMode DurationMode => durationMode;
    public bool IsPermanent => durationMode == BuffDurationMode.Permanent;
    public float Duration => Mathf.Max(0f, duration);
    public float TickInterval => Mathf.Max(0f, tickInterval);
    public int MaxStacks => Mathf.Max(1, maxStacks);
    public BuffStackMode StackMode => stackMode;
    public BuffStatModifier[] StatModifiers =>
        statModifiers ?? Array.Empty<BuffStatModifier>();
    public BuffLifecycleEffect[] LifecycleEffects =>
        lifecycleEffects ?? Array.Empty<BuffLifecycleEffect>();
}
