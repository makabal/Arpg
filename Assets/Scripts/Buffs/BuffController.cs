using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class BuffController
{
    private readonly MonoBehaviour _owner;
    private readonly CharacterStatsRuntime _stats;
    private readonly List<BuffInstance> _active =
        new List<BuffInstance>();

    public IReadOnlyList<BuffInstance> Active => _active;

    public event Action<BuffInstance> BuffAdded;
    public event Action<BuffInstance> BuffChanged;
    public event Action<BuffInstance> BuffRemoved;

    public BuffController(
        MonoBehaviour owner,
        CharacterStatsRuntime stats = null)
    {
        _owner = owner;
        _stats = stats;
    }

    public bool Apply(BuffApplication application)
    {
        BuffDefinition definition = application.Definition;
        if (definition == null || _owner == null)
            return false;

        if (definition.StackMode != BuffStackMode.Independent)
        {
            BuffInstance existing = Find(definition.BuffId);
            if (existing != null)
            {
                if (definition.StackMode == BuffStackMode.Ignore)
                    return false;

                bool addStack =
                    definition.StackMode == BuffStackMode.AddStack;
                existing.Refresh(application, addStack);
                BuffChanged?.Invoke(existing);
                return true;
            }
        }

        var instance = new BuffInstance(_owner, _stats, application);
        _active.Add(instance);
        instance.Activate();
        BuffAdded?.Invoke(instance);
        return true;
    }

    public bool Remove(string buffId)
    {
        bool removed = false;

        for (int i = _active.Count - 1; i >= 0; i--)
        {
            if (_active[i].Definition == null ||
                _active[i].Definition.BuffId != buffId)
            {
                continue;
            }

            RemoveAt(i);
            removed = true;
        }

        return removed;
    }

    public int RemoveByTag(BuffTag tag)
    {
        int removed = 0;

        for (int i = _active.Count - 1; i >= 0; i--)
        {
            BuffDefinition definition = _active[i].Definition;
            if (definition == null || (definition.Tags & tag) == 0)
                continue;

            RemoveAt(i);
            removed++;
        }

        return removed;
    }

    public bool HasTag(BuffTag tag)
    {
        foreach (BuffInstance instance in _active)
        {
            if (instance.Definition != null &&
                (instance.Definition.Tags & tag) != 0)
            {
                return true;
            }
        }

        return false;
    }

    public void Tick(float deltaTime)
    {
        for (int i = _active.Count - 1; i >= 0; i--)
        {
            BuffInstance instance = _active[i];
            instance.Tick(deltaTime);

            if (instance.IsExpired)
                RemoveAt(i);
        }
    }

    public void Clear()
    {
        for (int i = _active.Count - 1; i >= 0; i--)
            RemoveAt(i);
    }

    private BuffInstance Find(string buffId)
    {
        if (string.IsNullOrWhiteSpace(buffId))
            return null;

        foreach (BuffInstance instance in _active)
        {
            if (instance.Definition != null &&
                instance.Definition.BuffId == buffId)
            {
                return instance;
            }
        }

        return null;
    }

    private void RemoveAt(int index)
    {
        BuffInstance instance = _active[index];
        _active.RemoveAt(index);
        instance.Deactivate();
        BuffRemoved?.Invoke(instance);
    }
}
