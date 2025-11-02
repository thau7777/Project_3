using UnityEngine;

[CreateAssetMenu(fileName = "New ContinuousVFX Settings", menuName = "Scriptable Objects/Flyweight/ContinuousVFX Settings")]
public class ContinuousVFXSettings : FlyweightSettings
{
    public override Flyweight Create() 
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;
        var continuousVFX = go.GetOrAdd<ContinuousVFX>(); 
        
        continuousVFX.settings = this; 

        return continuousVFX;
    }
}