using UnityEngine;

[CreateAssetMenu(fileName = "Flyweight Settings", menuName = "ScriptableObjects/Flyweight/Projectile Settings")]
public class SlashVFXSetting : FlyweightSettings
{
    public float despawnDelay;
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;

        ParticleSystem particleSystem = prefab.GetComponentInChildren<ParticleSystem>();
        despawnDelay = particleSystem != null ? particleSystem.main.duration : 1f;

        var flyweight = go.GetOrAdd<SlashVFX>();
        flyweight.settings = this;

        return flyweight;
    }
}
