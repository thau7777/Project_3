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
        if (f.transform.parent != null)
            f.transform.SetParent(null);
        base.OnRelease(f);
    }
}
