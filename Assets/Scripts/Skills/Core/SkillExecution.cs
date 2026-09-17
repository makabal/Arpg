public static class SkillExecution
{
    public static void ApplyEffects(SkillHitContext context)
    {
        foreach (SkillEffect effect in context.Cast.Definition.Effects)
            effect?.Apply(context);
    }
}
