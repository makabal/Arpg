using System;
using System.Collections.Generic;

public sealed class PlayerSkillEntry
{
    public SkillDefinition Definition { get; }
    public SkillRuntime Runtime { get; }

    public PlayerSkillEntry(SkillDefinition definition)
    {
        Definition = definition;
        Runtime = new SkillRuntime(definition);
    }
}

public sealed class PlayerSkillCollection
{
    private readonly Dictionary<SkillDefinition, PlayerSkillEntry>
        _entries = new Dictionary<SkillDefinition, PlayerSkillEntry>();

    public IEnumerable<PlayerSkillEntry> Entries => _entries.Values;

    public event Action<PlayerSkillEntry> SkillRegistered;

    public PlayerSkillEntry Register(SkillDefinition definition)
    {
        if (definition == null)
            return null;

        if (_entries.TryGetValue(definition, out PlayerSkillEntry entry))
            return entry;

        entry = new PlayerSkillEntry(definition);
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
}
