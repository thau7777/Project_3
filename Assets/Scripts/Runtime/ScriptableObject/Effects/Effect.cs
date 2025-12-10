using UnityEngine;
using System.Collections.Generic;

// Base ScriptableObject for all effects
[CreateAssetMenu(fileName = "New Effect", menuName = "Scriptable Objects/Effect/Base Effect")]
public class Effect : ScriptableObject
{
    public Sprite icon;
    public float duration;
    public bool isPermanent;
    public EffectType effectType;

    [Header("Stat Modifiers (Percentage)")]
    [Tooltip("Percentage modifier (e.g., 50 = +50%, -20 = -20%)")]
    public float healthModifierPercent = 0;
    public float speedModifierPercent = 0;
    public float damageModifierPercent = 0;
    public float defenseModifierPercent = 0;

    [Header("Visual Feedback")]
    public Color effectColor = Color.white;
    public FlyweightSettings vfxSettings;

    public virtual void OnApply(GameObject target)
    {
        Debug.Log($"Applied {name} to {target.name}");

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
        Debug.Log($"Removed {name} from {target.name}");

        GetVfxFlyweightOnTarget(target)?.ReturnToPool();
        GetVfxFlyweightOnTarget(target)?.transform.SetParent(GameObject.Find("VFXStorage").transform);

    }
    public Flyweight GetVfxFlyweightOnTarget(GameObject target)
    {
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