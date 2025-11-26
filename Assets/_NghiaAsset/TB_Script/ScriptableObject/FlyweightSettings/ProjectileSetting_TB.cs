using Turnbase;
using UnityEngine;

[CreateAssetMenu(fileName = " New Projecttile Settings", menuName = "Scriptable Objects/Flyweight Turnbase /Projecttile Settings")]
public class ProjectileSetting_TB : FlyweightSettings_TB
{
    public override Flyweight_TB Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;
        var flyweight = go.GetOrAdd<ProjectileTurnBase>();
        flyweight.settings = this;
        return flyweight;
    }
}
