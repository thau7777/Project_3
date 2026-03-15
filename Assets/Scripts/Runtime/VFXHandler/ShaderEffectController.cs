using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Controls shader effects on multiple renderers using VFXMaterialData scriptable objects
/// </summary>
public class ShaderEffectController : MonoBehaviour
{
    [Header("VFX Material Data")]
    [Tooltip("List of VFX Material Data assets to use for effects")]
    public List<VFXMaterialData> vfxMaterialDataList = new List<VFXMaterialData>();

    [Header("References")]
    [Tooltip("List of renderer components to apply effects to (auto-assigned if empty)")]
    public List<Renderer> targetRenderers = new List<Renderer>();

    // Internal state
    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private Dictionary<Renderer, Material> currentEffectMaterials = new Dictionary<Renderer, Material>();
    private Coroutine activeCoroutine;

    private void Awake()
    {
        // Auto-assign renderers if list is empty
        if (targetRenderers.Count == 0)
        {
            Renderer[] renderer = GetComponentsInChildren<Renderer>();
            if (renderer != null)
                targetRenderers = renderer.ToList();
        }

        // Store original materials for each renderer
        foreach (var renderer in targetRenderers)
        {
            if (renderer != null)
            {
                originalMaterials[renderer] = renderer.sharedMaterial;
            }
        }
    }

    private void OnEnable()
    {
        RestoreOriginalMaterials();
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
        Debug.Log(gameObject.name);
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

        if (targetRenderers.Count == 0)
        {
            Debug.LogError("Target renderers list is empty!" + gameObject.name);
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
    /// Stop the currently playing effect and restore original materials
    /// </summary>
    public void StopEffect()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        RestoreOriginalMaterials();
    }

    private IEnumerator PlayEffectCoroutine(VFXMaterialData data)
    {
        // Create instance of effect material for each renderer
        foreach (var renderer in targetRenderers)
        {
            if (renderer == null) continue;

            Material effectMaterial = new Material(data.effectMaterial);
            currentEffectMaterials[renderer] = effectMaterial;

            // Copy textures if enabled
            if (data.copyTextures && originalMaterials.ContainsKey(renderer) && originalMaterials[renderer] != null)
            {
                CopyTextures(originalMaterials[renderer], effectMaterial, data.textureMappings);
            }

            // Apply effect material to renderer
            renderer.material = effectMaterial;
        }

        // Animate the shader property
        float elapsed = 0f;
        while (elapsed < data.duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / data.duration);
            float curveValue = data.curve.Evaluate(t);
            float propertyValue = Mathf.Lerp(data.startValue, data.endValue, curveValue);

            // Update property on all effect materials
            foreach (var kvp in currentEffectMaterials)
            {
                if (kvp.Value != null)
                {
                    kvp.Value.SetFloat(data.propertyName, propertyValue);
                }
            }

            yield return null;
        }

        // Ensure final value is set on all materials
        foreach (var kvp in currentEffectMaterials)
        {
            if (kvp.Value != null)
            {
                kvp.Value.SetFloat(data.propertyName, data.endValue);
            }
        }

        // Restore original materials if requested
        if (data.restoreOnComplete)
        {
            RestoreOriginalMaterials();
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

    private void RestoreOriginalMaterials()
    {
        foreach (var renderer in targetRenderers)
        {
            if (renderer != null && originalMaterials.ContainsKey(renderer) && originalMaterials[renderer] != null)
            {
                renderer.material = originalMaterials[renderer];
            }
        }

        // Clean up instanced materials
        foreach (var kvp in currentEffectMaterials)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        currentEffectMaterials.Clear();
    }

    private void OnDestroy()
    {
        // Clean up all instanced materials
        foreach (var kvp in currentEffectMaterials)
        {
            if (kvp.Value != null)
            {
                Destroy(kvp.Value);
            }
        }
        currentEffectMaterials.Clear();
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

    /// <summary>
    /// Add a renderer to the target list
    /// </summary>
    public void AddRenderer(Renderer renderer)
    {
        if (renderer != null && !targetRenderers.Contains(renderer))
        {
            targetRenderers.Add(renderer);
            originalMaterials[renderer] = renderer.sharedMaterial;
        }
    }

    /// <summary>
    /// Remove a renderer from the target list
    /// </summary>
    public void RemoveRenderer(Renderer renderer)
    {
        if (renderer != null)
        {
            targetRenderers.Remove(renderer);

            if (originalMaterials.ContainsKey(renderer))
                originalMaterials.Remove(renderer);

            if (currentEffectMaterials.ContainsKey(renderer))
            {
                if (currentEffectMaterials[renderer] != null)
                    Destroy(currentEffectMaterials[renderer]);
                currentEffectMaterials.Remove(renderer);
            }
        }
    }

    /// <summary>
    /// Get the number of target renderers
    /// </summary>
    public int GetRendererCount()
    {
        return targetRenderers.Count;
    }

    #endregion
}