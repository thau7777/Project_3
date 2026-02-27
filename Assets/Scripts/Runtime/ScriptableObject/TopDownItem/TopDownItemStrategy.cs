using UnityEngine;

[CreateAssetMenu(fileName = "New TopDown Item", menuName = "Scriptable Objects/StrategyPattern/TopDown Item")]
public class TopDownItemStrategy : ScriptableObject , IStrategy
{
    public Sprite itemIcon;
    public string itemName;
    public string itemDescription;
    public bool loseQuantityOnUse;
    public float coolDown;

    public EffectData effectToApply;
    public void Execute(IStrategyContext context)
    {
        ItemStrategyContext itemContext = context as ItemStrategyContext;
        ApplyBuffOrDeBuffToUser(itemContext.origin.gameObject);
    }
    private void ApplyBuffOrDeBuffToUser(GameObject user)
    {
        if (effectToApply.effect == null)
        {
            Debug.LogWarning("No effects assigned to BuffOrDeBuffOnSingleTarget skill!");
            return;
        }
        EffectsManager manager = user.GetOrAdd<EffectsManager>();
        manager.AddEffect(effectToApply);
    }
}
