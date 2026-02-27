using UnityEngine;

[CreateAssetMenu(fileName = "EffectIconSettings", menuName = "Scriptable Objects/Flyweight/EffectIcon Settings", order = 1)]
public class EffectIconSettings : FlyweightSettings
{
    public override Flyweight Create()
    {
        GameObject go = Instantiate(prefab);
        EffectIcon flyweight = go.GetComponent<EffectIcon>();
        flyweight.settings = this;
        return flyweight;
    }
}
