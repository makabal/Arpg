using System;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStat
{
    AttackDamage,
    MoveSpeed
}

public enum StatModifierOperation
{
    Add,
    Percent,
    Override
}

public sealed class CharacterStatsRuntime
{
    private sealed class RuntimeModifier
    {
        public object Source;
        public CharacterStat Stat;
        public StatModifierOperation Operation;
        public float Value;
    }

    private readonly CharacterStatsData _baseStats;
    private readonly List<RuntimeModifier> _modifiers =
        new List<RuntimeModifier>();

    public event Action<CharacterStat, float> Changed;

    public CharacterStatsRuntime(CharacterStatsData baseStats)
    {
        _baseStats = baseStats;
    }

    public float GetValue(CharacterStat stat)
    {
        float baseValue = GetBaseValue(stat);
        float flat = 0f;
        float percent = 0f;
        bool hasOverride = false;
        float overrideValue = 0f;

        foreach (RuntimeModifier modifier in _modifiers)
        {
            if (modifier.Stat != stat)
                continue;

            switch (modifier.Operation)
            {
                case StatModifierOperation.Add:
                    flat += modifier.Value;
                    break;
                case StatModifierOperation.Percent:
                    percent += modifier.Value;
                    break;
                case StatModifierOperation.Override:
                    hasOverride = true;
                    overrideValue = modifier.Value;
                    break;
            }
        }

        float value = hasOverride
            ? overrideValue
            : (baseValue + flat) * (1f + percent);

        return Mathf.Max(0f, value);
    }

    public int GetIntValue(CharacterStat stat)
    {
        return Mathf.RoundToInt(GetValue(stat));
    }

    public void AddModifier(
        object source,
        CharacterStat stat,
        StatModifierOperation operation,
        float value)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        _modifiers.Add(new RuntimeModifier
        {
            Source = source,
            Stat = stat,
            Operation = operation,
            Value = value
        });

        Changed?.Invoke(stat, GetValue(stat));
    }

    public void RemoveModifiers(object source)
    {
        if (source == null)
            return;

        var changedStats = new HashSet<CharacterStat>();

        for (int i = _modifiers.Count - 1; i >= 0; i--)
        {
            if (!ReferenceEquals(_modifiers[i].Source, source))
                continue;

            changedStats.Add(_modifiers[i].Stat);
            _modifiers.RemoveAt(i);
        }

        foreach (CharacterStat stat in changedStats)
            Changed?.Invoke(stat, GetValue(stat));
    }

    private float GetBaseValue(CharacterStat stat)
    {
        if (_baseStats == null)
            return 0f;

        switch (stat)
        {
            case CharacterStat.AttackDamage:
                return _baseStats.AttackDamage;
            case CharacterStat.MoveSpeed:
                return _baseStats.MoveSpeed;
            default:
                return 0f;
        }
    }
}
