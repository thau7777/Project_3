using UnityEngine;

[CreateAssetMenu(fileName = " New SkillIndicator Settings", menuName = "Scriptable Objects/Flyweight/SkillIndicator Settings")]
public class SkillIndicatorSettings : FlyweightSettings
{
    public bool isCircle;
    public LayerMask groundMask = ~0; // all layers by default
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;

        var flyweight = go.GetOrAdd<SkillIndicator>();
        flyweight.settings = this;

        return flyweight;
    }
}

