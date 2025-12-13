using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Controls shader effects on a renderer using VFXMaterialData scriptable objects
/// </summary>
public class ShaderEffectController : MonoBehaviour
{
    [Header("VFX Material Data")]
    [Tooltip("List of VFX Material Data assets to use for effects")]
    public List<VFXMaterialData> vfxMaterialDataList = new List<VFXMaterialData>();

    [Header("References")]
    [Tooltip("The renderer component (auto-assigned if null)")]
    public Renderer targetRenderer;

    // Internal state
    private Material originalMaterial;
    private Material currentEffectMaterial;
    private Coroutine activeCoroutine;

    private void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponent<Renderer>();

        if (targetRenderer != null)
            originalMaterial = targetRenderer.sharedMaterial;
    }
    private void OnEnable()
    {
        RestoreOriginalMaterial();
    }

    /// <summary>
    /// Play a VFX effect using the data at the specified index
    /// </summary>
    /// <param name="index">Index in the vfxMaterialDataList</param>
    public void PlayEffect(int index)
    {
        if (index < 0 || index >= vfxMaterialDataList.Count)
        {
            Debug.LogError($"Invalid VFX index {index}. List contains {vfxMaterialDataList.Count} elements.");
            return;
        }

        VFXMaterialData data = vfxMaterialDataList[index];
        if (data == null)
        {
            Debug.LogError($"VFXMaterialData at index {index} is null!");
            return;
        }

        PlayEffectWithData(data);
    }

    /// <summary>
    /// Play a VFX effect using specific VFXMaterialData
    /// </summary>
    /// <param name="data">The VFX material data to use</param>
    public void PlayEffectWithData(VFXMaterialData data)
    {
        if (data == null)
        {
            Debug.LogError("VFXMaterialData is null!");
            return;
        }

        if (data.effectMaterial == null)
        {
            Debug.LogError($"Effect material in {data.name} is null!");
            return;
        }

        if (targetRenderer == null)
        {
            Debug.LogError("Target renderer is null!");
            return;
        }

        // Stop any active effect
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        // Start the effect
        activeCoroutine = StartCoroutine(PlayEffectCoroutine(data));
    }

    /// <summary>
    /// Stop the currently playing effect and restore original material
    /// </summary>
    public void StopEffect()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        RestoreOriginalMaterial();
    }

    private IEnumerator PlayEffectCoroutine(VFXMaterialData data)
    {
        // Create instance of effect material
        currentEffectMaterial = new Material(data.effectMaterial);

        // Copy textures if enabled
        if (data.copyTextures && originalMaterial != null)
        {
            CopyTextures(originalMaterial, currentEffectMaterial, data.textureMappings);
        }

        // Apply effect material to renderer
        targetRenderer.material = currentEffectMaterial;

        // Animate the shader property
        float elapsed = 0f;
        while (elapsed < data.duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / data.duration);
            float curveValue = data.curve.Evaluate(t);
            float propertyValue = Mathf.Lerp(data.startValue, data.endValue, curveValue);

            currentEffectMaterial.SetFloat(data.propertyName, propertyValue);

            yield return null;
        }

        // Ensure final value is set
        currentEffectMaterial.SetFloat(data.propertyName, data.endValue);

        // Restore original material if requested
        if (data.restoreOnComplete)
        {
            RestoreOriginalMaterial();
        }

        activeCoroutine = null;
    }

    private void CopyTextures(Material source, Material destination, TextureMapping[] mappings)
    {
        if (mappings == null || mappings.Length == 0)
            return;

        foreach (var mapping in mappings)
        {
            if (string.IsNullOrEmpty(mapping.originalProperty) || string.IsNullOrEmpty(mapping.effectProperty))
                continue;

            if (source.HasProperty(mapping.originalProperty))
            {
                Texture texture = source.GetTexture(mapping.originalProperty);
                if (texture != null && destination.HasProperty(mapping.effectProperty))
                {
                    destination.SetTexture(mapping.effectProperty, texture);
                }
            }
        }
    }

    private void RestoreOriginalMaterial()
    {
        if (targetRenderer != null && originalMaterial != null)
        {
            targetRenderer.material = originalMaterial;
        }

        if (currentEffectMaterial != null)
        {
            Destroy(currentEffectMaterial);
            currentEffectMaterial = null;
        }
    }

    private void OnDestroy()
    {
        // Clean up instanced material
        if (currentEffectMaterial != null)
        {
            Destroy(currentEffectMaterial);
        }
    }

    #region Public Helper Methods

    /// <summary>
    /// Get the number of VFX data in the list
    /// </summary>
    public int GetVFXCount()
    {
        return vfxMaterialDataList.Count;
    }

    /// <summary>
    /// Get VFX data by index
    /// </summary>
    public VFXMaterialData GetVFXData(int index)
    {
        if (index >= 0 && index < vfxMaterialDataList.Count)
            return vfxMaterialDataList[index];
        return null;
    }

    /// <summary>
    /// Check if an effect is currently playing
    /// </summary>
    public bool IsPlaying()
    {
        return activeCoroutine != null;
    }

    #endregion
}