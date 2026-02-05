using UnityEngine;

[CreateAssetMenu(fileName = " New ContinousVFX Settings", menuName = "Scriptable Objects/Flyweight/ContinousVFX Settings")]
public class ContinousVFXSettings : FlyweightSettings
{
    [field: SerializeField]
    public float DefaultSize { get; private set; } = 1;
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;


        var flyweight = go.GetOrAdd<ContinousVFX>();
        flyweight.settings = this;
        return flyweight;
    }
    public override void OnGet(Flyweight f)
    {

    }
    public override void OnRelease(Flyweight f)
    {
        Transform vfxStorage = GameObject.Find("VFXStorage").transform;
        f.transform.SetParent(vfxStorage);
        base.OnRelease(f);
    }
}
