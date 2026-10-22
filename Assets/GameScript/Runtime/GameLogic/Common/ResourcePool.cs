using System;

public sealed class ResourcePool
{
    public int MaxValue { get; }
    public int CurrentValue { get; private set; }

    public event Action<int, int> Changed;

    public ResourcePool(int maxValue)
    {
        MaxValue = Math.Max(0, maxValue);
        CurrentValue = MaxValue;
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0)
            return true;

        if (CurrentValue < amount)
            return false;

        CurrentValue -= amount;
        Changed?.Invoke(CurrentValue, MaxValue);
        return true;
    }

    public void Restore(int amount)
    {
        if (amount <= 0)
            return;

        int previousValue = CurrentValue;
        CurrentValue = Math.Min(CurrentValue + amount, MaxValue);

        if (CurrentValue != previousValue)
            Changed?.Invoke(CurrentValue, MaxValue);
    }
}