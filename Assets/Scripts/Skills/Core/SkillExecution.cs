public static class SkillExecution
{
    public static void ApplyEffects(SkillHitContext context)
    {
        foreach (SkillEffect effect in context.Cast.Build.Effects)
            effect?.Apply(context);
    }
}
