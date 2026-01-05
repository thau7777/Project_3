using UnityEngine;

/// <summary>
/// Mapping between original material property and effect material property
/// </summary>
[System.Serializable]
public class TextureMapping
{
    [Tooltip("Property name in the original material (e.g., '_BaseMap' for URP)")]
    public string originalProperty = "_BaseMap";

    [Tooltip("Property name in the effect material (e.g., '_MainTex')")]
    public string effectProperty = "_MainTex";

    public TextureMapping(string original, string effect)
    {
        originalProperty = original;
        effectProperty = effect;
    }
}
[CreateAssetMenu(fileName = "New VFX Material Data", menuName = "Scriptable Objects/VFX Material Data" )]
public class VFXMaterialData : ScriptableObject
{
    [Header("Effect Material")]
    [Tooltip("The material with your shader graph effect (e.g., DissolveMaterial)")]
    public Material effectMaterial;

    [Header("Texture Transfer")]
    [Tooltip("Copy textures from original material to effect material")]
    public bool copyTextures = true;
    [Tooltip("Map textures: Original Property → Effect Property")]
    [SerializeField]
    public TextureMapping[] textureMappings = new TextureMapping[]
    {
        new TextureMapping("_BaseMap", "_MainTex"),        // URP Base Color
        new TextureMapping("_BumpMap", "_BumpMap"),        // Normal Map
        new TextureMapping("_EmissionMap", "_EmissionMap") // Emission
    };

    [Header("Effect Settings")]
    [Tooltip("The shader property name (e.g., '_DissolveAmount', '_FadeValue')")]
    public string propertyName = "_DissolveAmount";

    [Tooltip("Starting value of the effect")]
    public float startValue = 0f;

    [Tooltip("Ending value of the effect")]
    public float endValue = 1f;

    [Tooltip("Duration of the effect in seconds")]
    public float duration = 1f;

    [Tooltip("Animation curve for custom easing")]
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Material Management")]
    [Tooltip("Automatically restore original material when effect completes")]
    public bool restoreOnComplete = false;
}
