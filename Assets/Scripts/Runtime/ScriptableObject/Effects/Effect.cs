using UnityEngine;
using System.Collections.Generic;

// Base ScriptableObject for all effects
[CreateAssetMenu(fileName = "New Effect", menuName = "Scriptable Objects/Effect/Base Effect")]
public class Effect : ScriptableObject
{
    public Sprite icon;
    public bool isPermanent;
    public EffectType effectType;
    public float durationOnApply = 5f;

    public bool isStackable;
    [ShowIf("isStackable")]
    [Range(2, 10)]
    public int stackRequired = 3;

    [TabGroup("Stat Modifiers (Percentage)")]
    public float healthModifierPercent = 0;
    [TabGroup("Stat Modifiers (Percentage)")]
    public float speedModifierPercent = 0;
    [TabGroup("Stat Modifiers (Percentage)")]
    public float damageModifierPercent = 0;
    [TabGroup("Stat Modifiers (Percentage)")]
    public float defenseModifierPercent = 0;

    [TabGroup("Visual Feedback")]
    public Vector3 positionOffset;
    [TabGroup("Visual Feedback")]
    public ContinousVFXSettings vfxSettings;

    public virtual void OnApply(GameObject target)
    {
        // Spawn particle effect if exists
        if (vfxSettings != null)
        {
            var vfx = FlyweightFactory.Spawn(vfxSettings);

            vfx.FlyweightInitialize(target.transform.AddLocal(positionOffset.x,positionOffset.y,positionOffset.z), Quaternion.identity);
            (vfx as ContinousVFX).InitializeVFX(vfxSettings.DefaultSize, target.transform);
            vfx.gameObject.name = vfxSettings.prefab.name;

        }
        if (healthModifierPercent != 0)
        {

        }
        if (speedModifierPercent != 0)
        {

        }
        if (damageModifierPercent != 0)
        {

        }
        if(defenseModifierPercent != 0)
        {

        }
    }

    public virtual void OnUpdate(GameObject target, float deltaTime)
    {
        // Called each frame while effect is active
    }

    public virtual void OnRemove(GameObject target)
    {
        GetVfxFlyweightOnTarget(target)?.ReturnToPool();
        GetVfxFlyweightOnTarget(target)?.transform.SetParent(GameObject.Find("VFXStorage").transform);

    }
    public Flyweight GetVfxFlyweightOnTarget(GameObject target)
    {
        if (!vfxSettings) return null;
        Transform foundTransform = target.transform.Find(vfxSettings.prefab.name);
        if (foundTransform == null)
            return null;

        return foundTransform.GetComponent<Flyweight>();
    }
    // Check if this effect has the same stat modifiers as another
    public virtual bool HasSameModifiers(Effect other)
    {
        return Mathf.Approximately(healthModifierPercent, other.healthModifierPercent) &&
               Mathf.Approximately(speedModifierPercent, other.speedModifierPercent) &&
               Mathf.Approximately(damageModifierPercent, other.damageModifierPercent) &&
               Mathf.Approximately(defenseModifierPercent, other.defenseModifierPercent);
    }
}

public enum EffectType
{
    Buff,
    Debuff,
    Neutral
}

[System.Serializable]
public struct EffectData
{
    public Effect effect;
    public float duration;
    public int stacksToApply;
}
