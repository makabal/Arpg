using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Skill",
    menuName = "ARPG/Skills/Skill Definition")]
public sealed class SkillDefinition : ScriptableObject
{
    [Header("基本信息")]
    [SerializeField] private string skillId;
    [SerializeField] private string displayName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private SkillCategory categories;
    [SerializeField] private SkillActivationType activationType;

    [Header("消耗与冷却")]
    [SerializeField, Min(0)] private int manaCost;
    [SerializeField, Min(0f)] private float manaCostPerSecond;
    [SerializeField, Min(0f)] private float cooldown;
    [SerializeField, Min(0f)] private float castRange;

    [Header("释放")]
    [SerializeField] private SkillCastSettings castSettings = new SkillCastSettings();
    [SerializeField] private SkillAimResolver aimResolver;
    [SerializeField] private SkillDelivery delivery;
    [SerializeField] private SkillCondition[] conditions;

    [Header("效果")]
    [SerializeField] private SkillEffect[] effects;

    [Header("被动触发")]
    [SerializeField] private SkillPassiveTrigger passiveTrigger;

    [Header("表现，可暂时留空")]
    [SerializeField] private SkillPresentationData presentation = new SkillPresentationData();

    public string SkillId => skillId;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite Icon => icon;
    public SkillCategory Categories => categories;
    public SkillActivationType ActivationType => activationType;
    public int ManaCost => manaCost;
    public float ManaCostPerSecond => manaCostPerSecond;
    public float Cooldown => cooldown;
    public float CastRange => castRange;
    public SkillCastSettings CastSettings => castSettings;
    public SkillAimResolver AimResolver => aimResolver;
    public SkillDelivery Delivery => delivery;
    public IReadOnlyList<SkillCondition> Conditions =>
        conditions ?? Array.Empty<SkillCondition>();
    public IReadOnlyList<SkillEffect> Effects =>
        effects ?? Array.Empty<SkillEffect>();
    public SkillPassiveTrigger PassiveTrigger => passiveTrigger;
    public SkillPresentationData Presentation => presentation;
}
