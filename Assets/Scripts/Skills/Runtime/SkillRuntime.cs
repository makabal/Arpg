using System;
using UnityEngine;

public sealed class SkillRuntime
{
    public SkillDefinition Definition { get; }
    public float RemainingCooldown { get; private set; }
    public float TotalCooldown { get; private set; }
    public bool IsReady => RemainingCooldown <= 0f;

    public event Action<float, float> CooldownChanged;

    public SkillRuntime(SkillDefinition definition)
    {
        Definition = definition;
    }

    public void Tick(float deltaTime)
    {
        if (RemainingCooldown <= 0f)
            return;

        float previousCooldown = RemainingCooldown;
        RemainingCooldown = Mathf.Max(
            0f,
            RemainingCooldown - deltaTime);

        if (!Mathf.Approximately(previousCooldown, RemainingCooldown))
        {
            CooldownChanged?.Invoke(
                RemainingCooldown,
                TotalCooldown);
        }
    }

    public void StartCooldown(float duration)
    {
        TotalCooldown = Mathf.Max(0f, duration);
        RemainingCooldown = TotalCooldown;

        CooldownChanged?.Invoke(
            RemainingCooldown,
            TotalCooldown);
    }
}
