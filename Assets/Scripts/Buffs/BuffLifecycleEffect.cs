using UnityEngine;

public readonly struct BuffContext
{
    public MonoBehaviour Target { get; }
    public MonoBehaviour Source { get; }
    public BuffInstance Instance { get; }

    public int StackCount => Instance != null
        ? Instance.StackCount
        : 0;
    public float Potency => Instance != null
        ? Instance.Potency
        : 1f;

    public BuffContext(
        MonoBehaviour target,
        MonoBehaviour source,
        BuffInstance instance)
    {
        Target = target;
        Source = source;
        Instance = instance;
    }
}

public abstract class BuffLifecycleEffect : ScriptableObject
{
    public virtual void OnApply(BuffContext context) { }
    public virtual void OnTick(BuffContext context) { }
    public virtual void OnRemove(BuffContext context) { }
}
