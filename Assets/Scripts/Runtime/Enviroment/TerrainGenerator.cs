using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    [Header("Ground")]
    public MeshRenderer groundRenderer;
    public int textureResolution = 256;
    public float noiseFrequency = 5f;
    public bool randomizeSeed = true;
    public int seed;

    [Header("Shader Sync")]
    [Range(0f, 1f)]
    public float grassThreshold = 0.5f;
    public float noiseScale = 0.01f;

    [Header("Spawn Groups")]
    public EnviromentSpawnData grass;
    public EnviromentSpawnData trees;
    public EnviromentSpawnData decorations;

    Texture2D noiseTex;
    Material groundMaterial;

    void Start()
    {
        groundMaterial = groundRenderer.material;

        if (randomizeSeed)
            seed = Random.Range(0, 100000);

        GenerateNoise();
        ApplyNoiseToMaterial();
        SpawnAllGroups();
    }

    void GenerateNoise()
    {
        if (noiseTex == null || noiseTex.width != textureResolution)
        {
            if (noiseTex != null) Destroy(noiseTex);
            noiseTex = new Texture2D(textureResolution, textureResolution, TextureFormat.RGB24, false);
            noiseTex.wrapMode = TextureWrapMode.Repeat;
            noiseTex.filterMode = FilterMode.Bilinear;
        }

        System.Random rng = new System.Random(seed);
        float offsetX = (float)rng.NextDouble() * 10000f;
        float offsetY = (float)rng.NextDouble() * 10000f;

        for (int y = 0; y < textureResolution; y++)
        {
            for (int x = 0; x < textureResolution; x++)
            {
                float nx = (float)x / textureResolution * noiseFrequency + offsetX;
                float ny = (float)y / textureResolution * noiseFrequency + offsetY;
                float noise = Mathf.PerlinNoise(nx, ny);
                noiseTex.SetPixel(x, y, new Color(noise, noise, noise));
            }
        }

        noiseTex.Apply();
    }

    void ApplyNoiseToMaterial()
    {
        if (groundMaterial == null) return;

        groundMaterial.SetTexture("_NoiseTex", noiseTex);
        groundMaterial.SetFloat("_NoiseThreshold", grassThreshold);
        groundMaterial.SetFloat("_NoiseScale", noiseScale);
    }

    void SpawnAllGroups()
    {
        SpawnGroup(grass);
        SpawnGroup(decorations);
        SpawnGroup(trees);
    }

    void SpawnGroup(EnviromentSpawnData group)
    {
        if (group == null || group.prefabs.Count == 0) return;

        Bounds bounds = groundRenderer.bounds;
        int successfulSpawns = 0;
        int attempts = 0;
        int maxAttempts = group.count * 3;

        while (successfulSpawns < group.count && attempts < maxAttempts)
        {
            attempts++;

            Vector3 randomXZ = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.max.y + 5f,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            if (!Physics.Raycast(randomXZ, Vector3.down, out RaycastHit hit, 10f))
                continue;

            float noiseValue = SampleNoise(hit.point);

            // Check if spawn location matches the terrain layer requirement
            bool isGrassArea = noiseValue >= grassThreshold;

            bool shouldSpawn = group.terrainLayer switch
            {
                TerrainLayer.GrassOnly => isGrassArea,
                TerrainLayer.DirtOnly => !isGrassArea,
                TerrainLayer.Both => true,
                _ => true
            };

            if (!shouldSpawn)
                continue;

            // Edge safety check: make sure surrounding area is also the same terrain type
            if (group.edgeSafetyRadius > 0f && group.terrainLayer != TerrainLayer.Both)
            {
                if (!CheckEdgeSafety(hit.point, group.edgeSafetyRadius, group.edgeCheckSamples, isGrassArea))
                    continue;
            }

            GameObject prefab = group.prefabs[Random.Range(0, group.prefabs.Count)];
            GameObject obj = Instantiate(prefab, hit.point, Quaternion.identity, transform);

            float scale = Random.Range(group.scaleRange.x, group.scaleRange.y);
            obj.transform.localScale = Vector3.one * scale;

            float rotY = Random.Range(group.rotationYRange.x, group.rotationYRange.y);
            obj.transform.rotation = Quaternion.Euler(0, rotY, 0);

            successfulSpawns++;
        }
    }

    /// <summary>
    /// Checks if the area around a point is all the same terrain type (all grass or all dirt)
    /// </summary>
    bool CheckEdgeSafety(Vector3 center, float radius, int samples, bool shouldBeGrass)
    {
        float angleStep = 360f / samples;

        for (int i = 0; i < samples; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius
            );

            Vector3 checkPoint = center + offset;
            float checkNoise = SampleNoise(checkPoint);
            bool isGrassAtCheckPoint = checkNoise >= grassThreshold;

            // If any surrounding point doesn't match the required terrain type, reject this spawn
            if (isGrassAtCheckPoint != shouldBeGrass)
                return false;
        }

        return true; // All surrounding points match the required terrain type
    }

    float SampleNoise(Vector3 worldPos)
    {
        if (noiseTex == null) return 0f;

        Bounds b = groundRenderer.bounds;
        float u = Mathf.InverseLerp(b.min.x, b.max.x, worldPos.x);
        float v = Mathf.InverseLerp(b.min.z, b.max.z, worldPos.z);
        return noiseTex.GetPixelBilinear(u, v).r;
    }

    void ClearSpawns()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(transform.GetChild(i).gameObject);
#else
            Destroy(transform.GetChild(i).gameObject);
#endif
        }
    }

    [Button]
    public void RegenerateTerrain()
    {
        ClearSpawns();

        if (randomizeSeed)
            seed = Random.Range(0, 100000);

        GenerateNoise();
        ApplyNoiseToMaterial();
        SpawnAllGroups();
    }

    void OnDestroy()
    {
        if (noiseTex != null)
            Destroy(noiseTex);
    }
}