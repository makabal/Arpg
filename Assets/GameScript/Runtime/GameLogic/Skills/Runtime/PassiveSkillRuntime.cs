using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PassiveSkillRuntime : IDisposable
{
    private readonly SkillRuntime _runtime;
    private readonly PlayerSkillProgression _progression;
    private IDisposable _binding;

    public PlayerManager Owner { get; }
    public SkillDefinition Definition => _runtime.Definition;

    public PassiveSkillRuntime(
        PlayerManager owner,
        SkillRuntime runtime,
        PlayerSkillProgression progression)
    {
        Owner = owner;
        _runtime = runtime;
        _progression = progression;
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
        SkillBuildSnapshot build = _progression != null
            ? _progression.BuildSkill(Definition)
            : new SkillBuildBuilder(Definition).Build();
        var castContext = new SkillCastContext(
            Owner,
            Definition,
            aim,
            build);

        foreach (SkillCondition condition in build.Conditions)
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

        _runtime.StartCooldown(build.GetNumeric(
            SkillNumericStat.Cooldown,
            Definition.Cooldown));
        return true;
    }

    public void Dispose()
    {
        _binding?.Dispose();
        _binding = null;
    }
}
