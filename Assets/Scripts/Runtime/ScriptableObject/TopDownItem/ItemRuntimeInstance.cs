using UnityEngine;

public class ItemRuntimeInstance
{
    public readonly TopDownItemStrategy Definition;
    public int SlotIndex;
    public float lastCastTime;
    public int currentQuantity;
    public bool IsEmpty => Definition != null && Definition.loseQuantityOnUse && currentQuantity <= 0;
    public ItemRuntimeInstance(TopDownItemStrategy item, int index, int quantity = 1)
    {
        SlotIndex = index;
        Definition = item;
        lastCastTime = item ? -item.coolDown : 0;
        currentQuantity = quantity;
    }
    public float CurrentCooldownRemaining => Mathf.Max(0, (lastCastTime + Definition.coolDown) - Time.time);
    public float CurrentCooldownNormalized => Mathf.Clamp01((Time.time - lastCastTime) / Definition.coolDown);
    public bool IsOnCooldown => CurrentCooldownRemaining > 0;

    public void MarkUsed()
    {
        lastCastTime = Time.time;
    }
    public void Cast(ItemStrategyContext context)
    {
        MarkUsed();
        Definition.Execute(context);
    }
}

public class ItemStrategyContext : IStrategyContext
{
    public Transform origin;
    public Vector3 positionOffset;
    public Quaternion rotationOffset;
    public ItemStrategyContext(Transform origin, Vector3 positionOffset, Quaternion rotationOffset)
    {
        this.origin = origin;
        this.positionOffset = positionOffset;
        this.rotationOffset = rotationOffset;
    }
}
