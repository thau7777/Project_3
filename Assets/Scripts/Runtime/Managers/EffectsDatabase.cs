using UnityEngine;

public class EffectsDatabase : Singleton<EffectsDatabase>
{
    [SerializeField]
    private Effect[] _effects;

    public Effect GetEffectByName(string effectName)
    {
        foreach (var effect in _effects)
        {
            if (effect.name == effectName)
                return effect;
        }
        Debug.LogWarning($"Effect with name {effectName} not found in database.");
        return null;
    }
}
