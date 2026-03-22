using UnityEngine;

[CreateAssetMenu(fileName = "VFXSpawnPreset", menuName = "Scriptable Objects/VFX/Spawn Preset")]
public class VFXSpawnPreset : ScriptableObject
{
    [Header("References")]
    public OneShotVFXSettings vfxSettings;

    [Header("Parameters")]
    public float size = 1.5f;
    public float lifetime = 0.7f;
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

}
