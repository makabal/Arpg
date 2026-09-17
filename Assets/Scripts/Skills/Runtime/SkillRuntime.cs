using UnityEngine;

public sealed class SkillRuntime
{
    public SkillDefinition Definition { get; }
    public float RemainingCooldown { get; private set; }
    public bool IsReady => RemainingCooldown <= 0f;

    public SkillRuntime(SkillDefinition definition)
    {
        Definition = definition;
    }

    public void Tick(float deltaTime)
    {
        if (RemainingCooldown <= 0f)
            return;

        RemainingCooldown = Mathf.Max(
            0f,
            RemainingCooldown - deltaTime);
    }

    public void StartCooldown()
    {
        RemainingCooldown = Definition != null
            ? Definition.Cooldown
            : 0f;
    }
}
