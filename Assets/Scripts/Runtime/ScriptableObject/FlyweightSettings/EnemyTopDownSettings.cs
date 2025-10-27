using UnityEngine;

[CreateAssetMenu(fileName = " NewEnemyTopDownSettings", menuName = "Scriptable Objects/Flyweight/Enemy TopDown Settings")]
public class EnemyTopDownSettings : FlyweightSettings
{
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;

        var flyweight = go.GetComponent<EnemyTopdownStateDriver>();
        flyweight.settings = this;

        return flyweight;
    }

    public override void OnGet(Flyweight f)
    {
        base.OnGet(f);
        f.GetComponent<Damageable>().Initialize(100);
    }
    public override void OnRelease(Flyweight f)
    {
        base.OnRelease(f);
        f.GetComponent<EnemyTopdownStateDriver>().ResetStateContext();
    }
}
