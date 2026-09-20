using System;
using System.Collections.Generic;

public sealed class PlayerSkillEntry
{
    public SkillDefinition Definition { get; }
    public SkillRuntime Runtime { get; }
    public bool IsUnlocked { get; private set; }

    public PlayerSkillEntry(
        SkillDefinition definition,
        bool isUnlocked)
    {
        Definition = definition;
        Runtime = new SkillRuntime(definition);
        IsUnlocked = isUnlocked;
    }

    internal bool SetUnlocked(bool isUnlocked)
    {
        if (IsUnlocked == isUnlocked)
            return false;

        IsUnlocked = isUnlocked;
        return true;
    }
}

public sealed class PlayerSkillCollection
{
    private readonly Dictionary<SkillDefinition, PlayerSkillEntry>
        _entries = new Dictionary<SkillDefinition, PlayerSkillEntry>();

    public IEnumerable<PlayerSkillEntry> Entries => _entries.Values;

    public event Action<PlayerSkillEntry> SkillRegistered;
    public event Action<PlayerSkillEntry> UnlockChanged;

    public PlayerSkillEntry Register(
        SkillDefinition definition,
        bool isUnlocked = true)
    {
        if (definition == null)
            return null;

        if (_entries.TryGetValue(definition, out PlayerSkillEntry entry))
            return entry;

        entry = new PlayerSkillEntry(definition, isUnlocked);
        _entries.Add(definition, entry);
        SkillRegistered?.Invoke(entry);
        return entry;
    }

    public PlayerSkillEntry Get(SkillDefinition definition)
    {
        if (definition == null)
            return null;

        return _entries.TryGetValue(definition, out PlayerSkillEntry entry)
            ? entry
            : null;
    }

    public bool SetUnlocked(
        SkillDefinition definition,
        bool isUnlocked)
    {
        PlayerSkillEntry entry = Get(definition);

        if (entry == null || !entry.SetUnlocked(isUnlocked))
            return false;

        UnlockChanged?.Invoke(entry);
        return true;
    }
}
