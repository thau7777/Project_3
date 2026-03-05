using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TopDown Item", menuName = "Scriptable Objects/StrategyPattern/TopDown Item")]
public class TopDownItemStrategy : ScriptableObject , IStrategy
{
    public Sprite itemIcon;
    public string itemName;
    public string itemDescription;
    public bool loseQuantityOnUse;
    public float coolDown;

    public List<EffectData> effectsToApply = new();
    public void Execute(IStrategyContext context)
    {
        ItemStrategyContext itemContext = context as ItemStrategyContext;
        ApplyEffectsToUser(itemContext.origin.gameObject);
    }
    private void ApplyEffectsToUser(GameObject user)
    {
        if (effectsToApply.Count == 0 || effectsToApply == null)
        {
            Debug.LogWarning("No effects assigned to this item");
            return;
        }
        EffectsManager manager = user.GetOrAdd<EffectsManager>();
        if (manager != null)
        {
            foreach (EffectData effect in effectsToApply)
            {
                manager.AddEffect(effect);
            }
        }
    }
}
