using UnityEngine;

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
