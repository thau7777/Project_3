using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New ContinuousVFX Settings", menuName = "Scriptable Objects/Flyweight/ContinuousVFX Settings")]
public class AdvanceOneShotVFXSettings : FlyweightSettings
{
    public string playEventName = "Play";
    public string durationName = "Duration";
    public string sizeName = "Size";

    public FlyweightSettings decalEffectSettings;
    public override Flyweight Create() 
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;
        var continuousVFX = go.GetOrAdd<AdvanceOneShotVFX>(); 
        
        continuousVFX.settings = this; 

        return continuousVFX;
    }
}