using UnityEngine;

[CreateAssetMenu(fileName = " New OneShotVFX Settings", menuName = "Scriptable Objects/Flyweight/OneShotVFX Settings")]
public class OneShotVFXSettings : FlyweightSettings
{
    [field:SerializeField]
    public float DespawnDelay { get; private set; }
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;

        ParticleSystem particleSystem = prefab.GetComponentInChildren<ParticleSystem>();
        DespawnDelay = particleSystem != null ? particleSystem.main.duration : 1f;

        var flyweight = go.GetOrAdd<OneShotVFX>();
        flyweight.settings = this;

        return flyweight;
    }
}
