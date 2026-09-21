using System.Collections.Generic;
using UnityEngine;

public sealed class SkillBuildSnapshot
{
    private readonly Dictionary<SkillNumericStat, NumericAdjustment>
        _numericAdjustments;

    public SkillDefinition Definition { get; }
    public SkillAimResolver AimResolver { get; }
    public SkillDelivery Delivery { get; }
    public SkillTargetResolver TargetResolverOverride { get; }
    public IReadOnlyList<SkillEffect> Effects { get; }
    public IReadOnlyList<SkillCondition> Conditions { get; }
    public SkillPresentationData Presentation { get; }

    internal SkillBuildSnapshot(
        SkillDefinition definition,
        SkillAimResolver aimResolver,
        SkillDelivery delivery,
        SkillTargetResolver targetResolverOverride,
        List<SkillEffect> effects,
        List<SkillCondition> conditions,
        SkillPresentationData presentation,
        Dictionary<SkillNumericStat, NumericAdjustment> adjustments)
    {
        Definition = definition;
        AimResolver = aimResolver;
        Delivery = delivery;
        TargetResolverOverride = targetResolverOverride;
        Effects = effects;
        Conditions = conditions;
        Presentation = presentation;
        _numericAdjustments = adjustments;
    }

    public float GetNumeric(SkillNumericStat stat, float baseValue)
    {
        if (!_numericAdjustments.TryGetValue(
                stat,
                out NumericAdjustment adjustment))
        {
            return baseValue;
        }

        float value = adjustment.HasOverride
            ? adjustment.OverrideValue
            : (baseValue + adjustment.Flat) *
                (1f + adjustment.Percent);

        return Mathf.Max(0f, value);
    }

    public int GetInt(SkillNumericStat stat, int baseValue)
    {
        return Mathf.RoundToInt(GetNumeric(stat, baseValue));
    }

    internal readonly struct NumericAdjustment
    {
        public float Flat { get; }
        public float Percent { get; }
        public bool HasOverride { get; }
        public float OverrideValue { get; }

        public NumericAdjustment(
            float flat,
            float percent,
            bool hasOverride,
            float overrideValue)
        {
            Flat = flat;
            Percent = percent;
            HasOverride = hasOverride;
            OverrideValue = overrideValue;
        }
    }
}

public sealed class SkillBuildBuilder
{
    private sealed class MutableAdjustment
    {
        public float Flat;
        public float Percent;
        public bool HasOverride;
        public float OverrideValue;
    }

    private readonly SkillDefinition _definition;
    private readonly List<SkillEffect> _effects =
        new List<SkillEffect>();
    private readonly List<SkillCondition> _conditions =
        new List<SkillCondition>();
    private readonly Dictionary<SkillNumericStat, MutableAdjustment>
        _adjustments =
            new Dictionary<SkillNumericStat, MutableAdjustment>();

    private SkillAimResolver _aimResolver;
    private SkillDelivery _delivery;
    private SkillTargetResolver _targetResolverOverride;
    private SkillPresentationData _presentation;

    public SkillBuildBuilder(SkillDefinition definition)
    {
        _definition = definition;

        if (definition == null)
            return;

        _aimResolver = definition.AimResolver;
        _delivery = definition.Delivery;
        _presentation = definition.Presentation;

        foreach (SkillEffect effect in definition.Effects)
        {
            if (effect != null)
                _effects.Add(effect);
        }

        foreach (SkillCondition condition in definition.Conditions)
        {
            if (condition != null)
                _conditions.Add(condition);
        }
    }

    public void AddNumericModifier(
        SkillNumericStat stat,
        SkillNumericOperation operation,
        float value)
    {
        if (!_adjustments.TryGetValue(stat, out MutableAdjustment item))
        {
            item = new MutableAdjustment();
            _adjustments.Add(stat, item);
        }

        switch (operation)
        {
            case SkillNumericOperation.Add:
                item.Flat += value;
                break;
            case SkillNumericOperation.Percent:
                item.Percent += value;
                break;
            case SkillNumericOperation.Override:
                item.HasOverride = true;
                item.OverrideValue = value;
                break;
        }
    }

    public void SetAimResolver(SkillAimResolver resolver)
    {
        _aimResolver = resolver;
    }

    public void SetDelivery(SkillDelivery delivery)
    {
        _delivery = delivery;
    }

    public void SetTargetResolverOverride(SkillTargetResolver resolver)
    {
        _targetResolverOverride = resolver;
    }

    public void AddEffect(SkillEffect effect)
    {
        if (effect != null && !_effects.Contains(effect))
            _effects.Add(effect);
    }

    public void RemoveEffect(SkillEffect effect)
    {
        if (effect != null)
            _effects.Remove(effect);
    }

    public void AddCondition(SkillCondition condition)
    {
        if (condition != null && !_conditions.Contains(condition))
            _conditions.Add(condition);
    }

    public void SetPresentation(SkillPresentationData presentation)
    {
        _presentation = presentation;
    }

    public SkillBuildSnapshot Build()
    {
        var adjustments = new Dictionary<
            SkillNumericStat,
            SkillBuildSnapshot.NumericAdjustment>();

        foreach (KeyValuePair<SkillNumericStat, MutableAdjustment> pair in
                 _adjustments)
        {
            MutableAdjustment value = pair.Value;
            adjustments.Add(
                pair.Key,
                new SkillBuildSnapshot.NumericAdjustment(
                    value.Flat,
                    value.Percent,
                    value.HasOverride,
                    value.OverrideValue));
        }

        return new SkillBuildSnapshot(
            _definition,
            _aimResolver,
            _delivery,
            _targetResolverOverride,
            new List<SkillEffect>(_effects),
            new List<SkillCondition>(_conditions),
            _presentation,
            adjustments);
    }
}
