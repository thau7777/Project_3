using UnityEngine;

[CreateAssetMenu(fileName = " New Decal Settings", menuName = "Scriptable Objects/Flyweight/Decal Settings")]
public class DecalProjectorSettings : FlyweightSettings
{
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;
        var decalProjectorController = go.GetOrAdd<DecalProjectorController>();

        decalProjectorController.settings = this;

        return decalProjectorController;
    }
}
