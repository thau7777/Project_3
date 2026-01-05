using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OneShotVFXSpawner : MonoBehaviour
{
    [Header("Presets")]
    [SerializeField]
    private List<VFXSpawnPreset> _presets;

    [Header("Spawn Points")]
    [SerializeField]
    private List<Transform> _spawnPoints;

    /// <summary>
    /// Animation Event - spawn by preset name
    /// Example: "SlashEffect" or "ImpactHeavy"
    /// </summary>
    public void SpawnVFXByName(string presetName)
    {
        var preset = _presets.Find(p => p.name == presetName);

        if (preset == null)
        {
            Debug.LogWarning($"[PresetBasedVFXSpawner] Preset not found: {presetName}");
            return;
        }

        SpawnFromPreset(preset, 0); // Default to spawn point 0
    }

    /// <summary>
    /// Animation Event - spawn by preset index and spawn point
    /// encodedValue = presetIndex * 100 + spawnPointIndex
    /// </summary>
    public void SpawnVFX(int encodedValue)
    {
        int presetIndex = encodedValue / 100;
        int spawnPointIndex = encodedValue % 100;

        if (presetIndex < 0 || presetIndex >= _presets.Count)
        {
            Debug.LogWarning($"[PresetBasedVFXSpawner] Invalid preset index: {presetIndex}");
            return;
        }

        if (spawnPointIndex < 0 || spawnPointIndex >= _spawnPoints.Count)
        {
            Debug.LogWarning($"[PresetBasedVFXSpawner] Invalid spawn point index: {spawnPointIndex}");
            return;
        }

        SpawnFromPreset(_presets[presetIndex], spawnPointIndex);
    }

    /// <summary>
    /// Advanced string format: "presetName,spawnPointIndex"
    /// Example: "SlashEffect,1"
    /// </summary>
    public void SpawnVFXAdvanced(string encodedData)
    {
        var values = encodedData.Split(',');

        if (values.Length < 1)
        {
            Debug.LogWarning($"[PresetBasedVFXSpawner] Invalid format: {encodedData}");
            return;
        }

        string presetName = values[0];
        int spawnPointIndex = 0;

        if (values.Length > 1 && int.TryParse(values[1], out int parsedIndex))
            spawnPointIndex = parsedIndex;

        var preset = _presets.Find(p => p.name == presetName);

        if (preset == null)
        {
            Debug.LogWarning($"[PresetBasedVFXSpawner] Preset not found: {presetName}");
            return;
        }

        if (spawnPointIndex < 0 || spawnPointIndex >= _spawnPoints.Count)
        {
            Debug.LogWarning($"[PresetBasedVFXSpawner] Invalid spawn point index: {spawnPointIndex}");
            spawnPointIndex = 0;
        }

        SpawnFromPreset(preset, spawnPointIndex);
    }

    private void SpawnFromPreset(VFXSpawnPreset preset, int spawnPointIndex)
    {
        var spawnPoint = _spawnPoints[spawnPointIndex];

        var vfx = FlyweightFactory.Spawn(preset.vfxSettings);

        // Calculate spawn position and rotation with offsets
        Vector3 spawnPos = spawnPoint.position + spawnPoint.TransformDirection(preset.positionOffset);
        Quaternion spawnRot = spawnPoint.rotation * Quaternion.Euler(preset.rotationOffset);

        vfx.FlyweightInitialize(spawnPos, spawnRot);
        vfx.transform.SetParent(spawnPoint);

        // Apply VFX Graph parameters
        if (vfx.TryGetComponent(out VisualEffect vfxGraph))
        {
            if (vfxGraph.HasFloat("Size"))
                vfxGraph.SetFloat("Size", preset.size);

            if (vfxGraph.HasFloat("LifeTime"))
                vfxGraph.SetFloat("LifeTime", preset.lifetime);

            if (preset.useCustomColor && vfxGraph.HasVector4("Color"))
                vfxGraph.SetVector4("Color", preset.customColor);

            if (preset.useCustomSpeed && vfxGraph.HasFloat("Speed"))
                vfxGraph.SetFloat("Speed", preset.customSpeed);
        }

        // Apply to custom VFX component
        if (vfx is OneShotVFX oneShotVFX)
        {
            oneShotVFX.InitializeVFX(preset.size, preset.lifetime);
        }
    }

    // Editor helper to list available presets
#if UNITY_EDITOR
    [ContextMenu("List Available Presets")]
    private void ListPresets()
    {
        Debug.Log("=== Available VFX Presets ===");
        for (int i = 0; i < _presets.Count; i++)
        {
            Debug.Log($"[{i}] {_presets[i].name} - Size: {_presets[i].size}, Lifetime: {_presets[i].lifetime}");
        }
    }
#endif
}