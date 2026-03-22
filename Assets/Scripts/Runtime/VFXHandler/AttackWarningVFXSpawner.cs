using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AttackWarningVFXSpawner : MonoBehaviour
{
    [Header("Presets")]
    [SerializeField]
    private List<VFXSpawnPreset> _presets;

    [Header("Spawn Points")]
    [SerializeField]
    private List<Transform> _spawnPoints;

    /// <summary>
    /// Animation Event - "presetIndex,spawnPointIndex,parryAble"
    /// e.g. "2,1,1" = preset 2, spawn point 1, parryAble true
    /// e.g. "0,0,0" = preset 0, spawn point 0, parryAble false
    /// </summary>
    public void SpawnVFX(string encodedValue)
    {
        var parts = encodedValue.Split(',');
        if (parts.Length != 3)
        {
            Debug.LogWarning($"[OneShotVFXSpawner] Expected format \"preset,spawnPoint,parryAble\" but got: {encodedValue}");
            return;
        }

        if (!int.TryParse(parts[0], out int presetIndex) ||
            !int.TryParse(parts[1], out int spawnPointIndex) ||
            !int.TryParse(parts[2], out int parryableInt))
        {
            Debug.LogWarning($"[OneShotVFXSpawner] Failed to parse values from: {encodedValue}");
            return;
        }

        if (presetIndex < 0 || presetIndex >= _presets.Count)
        {
            Debug.LogWarning($"[OneShotVFXSpawner] Invalid preset index: {presetIndex}");
            return;
        }
        if (spawnPointIndex < 0 || spawnPointIndex >= _spawnPoints.Count)
        {
            Debug.LogWarning($"[OneShotVFXSpawner] Invalid spawn point index: {spawnPointIndex}");
            return;
        }

        SpawnFromPreset(_presets[presetIndex], spawnPointIndex, parryableInt == 1);
    }


    private void SpawnFromPreset(VFXSpawnPreset preset, int spawnPointIndex, bool parryAble)
    {
        var spawnPoint = _spawnPoints[spawnPointIndex];

        var vfx = FlyweightFactory.Spawn(preset.vfxSettings);

        // Calculate spawn position and rotation with offsets
        Vector3 spawnPos = spawnPoint.position + spawnPoint.TransformDirection(preset.positionOffset);
        Quaternion spawnRot = spawnPoint.rotation * Quaternion.Euler(preset.rotationOffset);

        vfx.FlyweightInitialize(spawnPos, spawnRot, spawnPoint);

        // Apply VFX Graph parameters
        if (vfx.TryGetComponent(out VisualEffect vfxGraph))
        {
            vfx.transform.localScale = Vector3.one * preset.size;

            if (vfxGraph.HasFloat("LifeTime"))
                vfxGraph.SetFloat("LifeTime", preset.lifetime);

            if(vfxGraph.HasBool("CanParry"))
                vfxGraph.SetBool("CanParry", parryAble);

        }

        // Apply to custom VFX component
        if (vfx is OneShotVFX oneShotVFX)
        {
            oneShotVFX.InitializeVFX(preset.size, preset.lifetime);
        }
    }


}