using UnityEngine;

[CreateAssetMenu(fileName = "New ImpactVFXSetting Settings", menuName = "Scriptable Objects/Flyweight/ImpactVFXSetting Settings")]
public class ImpactVFXSetting : FlyweightSettings
{
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;
        var impactVFX = go.GetOrAdd<ImpactVFX>();

        impactVFX.settings = this;

        return impactVFX;
    }
}