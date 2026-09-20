using System;

public enum SkillActivationType
{
    Active,
    Passive
}

public enum SkillSlot
{
    BasicAttack,
    Skill1,
    Skill2,
    Skill3,
    Skill4,
    Skill5
}

public enum SkillReleaseMode
{
    Timed,
    AnimationEvents
}

public enum SkillDurationMode
{
    Fixed,
    WhileInputHeld
}

public enum SkillCooldownStartMode
{
    OnCastStart,
    OnSkillEnd
}

public enum SkillAreaLifetimeMode
{
    FixedDuration,
    UntilSkillEnds
}

public enum SkillAnimationParameterType
{
    Trigger,
    Bool
}

[Flags]
public enum SkillCategory
{
    None = 0,
    Attack = 1 << 0,
    Defense = 1 << 1,
    Recovery = 1 << 2,
    Movement = 1 << 3,
    Control = 1 << 4,
    Summon = 1 << 5
}

public enum SkillTargetType
{
    Self,
    SelectedTarget,
    DirectionalArea,
    GroundArea
}

public enum SkillAreaShape
{
    Single,
    Circle,
    Cone,
    Line
}

public enum SkillAreaOrigin
{
    Caster,
    AimPosition
}

public enum SkillUseFailure
{
    None,
    InvalidSlot,
    NotEquipped,
    SkillLocked,
    PassiveSkill,
    PlayerDead,
    Busy,
    OnCooldown,
    NotEnoughMana,
    NoAimResolver,
    NoDelivery,
    NoValidTarget,
    OutOfRange,
    ConditionFailed
}
