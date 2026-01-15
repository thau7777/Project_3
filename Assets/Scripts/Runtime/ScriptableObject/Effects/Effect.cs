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

    [Header("Stat Modifiers (Percentage)")]
    [Tooltip("Percentage modifier (e.g., 50 = +50%, -20 = -20%)")]
    public float healthModifierPercent = 0;
    public float speedModifierPercent = 0;
    public float damageModifierPercent = 0;
    public float defenseModifierPercent = 0;

    [Header("Visual Feedback")]
    public FlyweightSettings vfxSettings;

    public virtual void OnApply(GameObject target)
    {
        // Spawn particle effect if exists
        if (vfxSettings != null)
        {
            var vfx = FlyweightFactory.Spawn(vfxSettings);
            vfx.transform.SetParent(target.transform);

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