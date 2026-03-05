using UnityEngine;

[CreateAssetMenu(fileName = "FloatingCombatTextSettings", menuName = "Scriptable Objects/Flyweight/FloatingCombatTextSettings")]
public class FloatingCombatTextSettings : FlyweightSettings
{
    public override Flyweight Create()
    {
        GameObject go = Instantiate(prefab);
        FloatingCombatText flyweight = go.GetComponent<FloatingCombatText>();
        flyweight.settings = this;
        return flyweight;
    }

}
