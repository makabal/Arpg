using System;

public enum HealthMode
{
    Normal,
    Infinite
}

public sealed class Health
{
    public int MaxHealth { get; }
    public int CurrentHealth { get; private set; }
    public HealthMode Mode { get; }
    public bool IsInfinite => Mode == HealthMode.Infinite;
    public bool IsDead => !IsInfinite && CurrentHealth <= 0;

    public event Action<int> Damaged;
    public event Action Died;
    public event Action<int, int> Changed;

    public Health(int maxHealth, HealthMode mode = HealthMode.Normal)
    {
        MaxHealth = Math.Max(1, maxHealth);
        CurrentHealth = MaxHealth;
        Mode = mode;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || damage <= 0)
            return;

        if (IsInfinite)
        {
            Damaged?.Invoke(damage);
            return;
        }

        int actualDamage = Math.Min(damage, CurrentHealth);
        CurrentHealth -= actualDamage;

        Damaged?.Invoke(actualDamage);
        Changed?.Invoke(CurrentHealth, MaxHealth);

        if (IsDead)
            Died?.Invoke();
    }

    public void Restore(int amount)
    {
        if (IsInfinite || IsDead || amount <= 0)
            return;

        int previousHealth = CurrentHealth;
        CurrentHealth = Math.Min(CurrentHealth + amount, MaxHealth);

        if (CurrentHealth != previousHealth)
            Changed?.Invoke(CurrentHealth, MaxHealth);
    }
}

public interface IDamageable
{
    Health Health { get; }
    void TakeDamage(int damage);
}
