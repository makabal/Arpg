using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PassiveSkillRuntime : IDisposable
{
    private readonly SkillRuntime _runtime;
    private IDisposable _binding;

    public PlayerManager Owner { get; }
    public SkillDefinition Definition => _runtime.Definition;

    public PassiveSkillRuntime(
        PlayerManager owner,
        SkillRuntime runtime)
    {
        Owner = owner;
        _runtime = runtime;
    }

    public void Enable()
    {
        if (_binding != null || Definition.PassiveTrigger == null)
            return;

        _binding = Definition.PassiveTrigger.Bind(this);
    }

    public bool TryTrigger(
        IReadOnlyList<SkillTarget> targets,
        Vector2 aimPosition,
        Vector2 direction)
    {
        if (!_runtime.IsReady || Owner.Health.IsDead)
            return false;

        var aim = new SkillAimData(
            aimPosition,
            direction);
        var castContext = new SkillCastContext(
            Owner,
            Definition,
            aim);

        foreach (SkillCondition condition in Definition.Conditions)
        {
            if (condition != null &&
                condition.Validate(castContext) != SkillUseFailure.None)
            {
                return false;
            }
        }

        var hitContext = new SkillHitContext(
            castContext,
            targets,
            aimPosition,
            Vector2.zero);

        SkillExecution.ApplyEffects(hitContext);

        _runtime.StartCooldown();
        return true;
    }

    public void Dispose()
    {
        _binding?.Dispose();
        _binding = null;
    }
}
