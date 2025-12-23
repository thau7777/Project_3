using UnityEngine;

[CreateAssetMenu(fileName = " New OneShotVFX Settings", menuName = "Scriptable Objects/Flyweight/OneShotVFX Settings")]
public class ContinousVFXSettings : FlyweightSettings
{
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;


        var flyweight = go.GetOrAdd<ContinousVFX>();
        flyweight.settings = this;
        return flyweight;
    }
    public override void OnRelease(Flyweight f)
    {
        Transform vfxStorage = GameObject.Find("VFXStorage").transform;
        f.transform.SetParent(vfxStorage);
        base.OnRelease(f);
    }
}
