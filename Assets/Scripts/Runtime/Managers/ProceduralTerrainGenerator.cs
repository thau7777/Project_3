using UnityEngine;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    [Header("Terrain Reference")]
    public Terrain terrain;

    [Header("Terrain Height (Optional)")]
    [Tooltip("Set to 0 for completely flat terrain")]
    [Range(0f, 0.1f)]
    public float flatHeight = 0f;

    [Header("Texture Noise Settings")]
    [Range(1f, 50f)]
    public float textureNoiseScale = 15f;
    public int seed = 0;

    [Header("Texture Layers (Assign in order)")]
    [Tooltip("0: Grass, 1: Dirt/Rock, 2: Sand/Path, etc.")]
    public TerrainLayer[] terrainLayers;

    [Header("Layer Distribution Settings")]
    [Tooltip("Percentage of terrain that will be grass (Layer 0)")]
    [Range(0f, 1f)]
    public float grassPercentage = 0.8f;

    [Header("Trees (Instantiated GameObjects)")]
    public GameObject[] treePrefabs;
    [Range(0, 100)]
    public int treeCount = 10;
    [Range(0.5f, 2f)]
    public float treeMinScale = 0.8f;
    [Range(0.5f, 2f)]
    public float treeMaxScale = 1.2f;
    [Tooltip("Spawn trees only on specific layer (0 = Grass)")]
    public int treeSpawnLayer = 0;
    [Range(0f, 1f)]
    public float treeLayerWeightThreshold = 0.5f;
    public Transform treeParent;

    [Header("Grass (Instantiated GameObjects)")]
    public GameObject[] grassPrefabs;
    [Range(0, 10000)]
    public int grassCount = 2000;
    [Range(0.1f, 5f)]
    public float grassMinScale = 0.5f;
    [Range(0.1f, 5f)]
    public float grassMaxScale = 1.5f;
    [Range(0f, 1f)]
    public float grassDensity = 0.7f;
    [Tooltip("Spawn grass only on specific layer (0 = Grass)")]
    public int grassSpawnLayer = 0;
    [Range(0f, 1f)]
    public float grassLayerWeightThreshold = 0.3f;
    public Transform grassParent;

    [Header("Decorations (Rocks, Bushes, etc.)")]
    public GameObject[] decorationPrefabs;
    [Range(0, 2000)]
    public int decorationCount = 500;
    [Range(0.1f, 10f)]
    public float decorationMinScale = 0.5f;
    [Range(0.1f, 10f)]
    public float decorationMaxScale = 2.0f;
    [Tooltip("Spawn decorations on specific layer (-1 = any layer)")]
    public int decorationSpawnLayer = -1;
    [Range(0f, 1f)]
    public float decorationLayerWeightThreshold = 0.5f;
    public Transform decorationParent;

    private TerrainData terrainData;
    private int heightmapResolution;
    private int alphamapResolution;
    private System.Random rng;

    void Start()
    {
        Debug.Log("=== Terrain Generator Starting ===");

        if (terrain == null)
            terrain = GetComponent<Terrain>();

        CleanupAllGenerated();

        terrainData = terrain.terrainData;
        if (terrainData == null)
        {
            Debug.LogError("Terrain has no TerrainData!");
            return;
        }

        heightmapResolution = terrainData.heightmapResolution;
        alphamapResolution = terrainData.alphamapResolution;

        Debug.Log($"Heightmap Resolution: {heightmapResolution}");
        Debug.Log($"Alphamap Resolution: {alphamapResolution}");

        seed = Random.Range(0, 100000);
        rng = new System.Random(seed);

        GenerateTerrain();
    }

    void CleanupAllGenerated()
    {
        Debug.Log("Cleaning up old generated content...");

        if (treeParent != null)
        {
            int treeChildCount = treeParent.childCount;
            while (treeParent.childCount > 0)
            {
                DestroyImmediate(treeParent.GetChild(0).gameObject);
            }
            Debug.Log($"Removed {treeChildCount} old trees");
        }

        if (grassParent != null)
        {
            int grassChildCount = grassParent.childCount;
            while (grassParent.childCount > 0)
            {
                DestroyImmediate(grassParent.GetChild(0).gameObject);
            }
            Debug.Log($"Removed {grassChildCount} old grass objects");
        }

        if (decorationParent != null)
        {
            int decorationChildCount = decorationParent.childCount;
            while (decorationParent.childCount > 0)
            {
                DestroyImmediate(decorationParent.GetChild(0).gameObject);
            }
            Debug.Log($"Removed {decorationChildCount} old decoration objects");
        }

        System.GC.Collect();
        Resources.UnloadUnusedAssets();

        Debug.Log("Cleanup complete!");
    }

    void GenerateTerrain()
    {
        Debug.Log("Step 1: Setting terrain height...");
        SetTerrainHeight();
        Debug.Log("Height set complete!");

        Debug.Log("Step 2: Painting textures based on noise...");
        PaintTexturesWithNoise();
        Debug.Log("Textures complete!");

        Debug.Log("Step 3: Spawning trees...");
        SpawnTrees();
        Debug.Log("Trees complete!");

        Debug.Log("Step 4: Spawning grass...");
        SpawnGrass();
        Debug.Log("Grass complete!");

        Debug.Log("Step 5: Spawning decorations...");
        SpawnDecorations();
        Debug.Log("Decorations complete!");

        Debug.Log("=== Terrain Generation Complete ===");
    }

    void SetTerrainHeight()
    {
        // Create a flat terrain or set uniform height
        float[,] heights = new float[heightmapResolution, heightmapResolution];

        for (int y = 0; y < heightmapResolution; y++)
        {
            for (int x = 0; x < heightmapResolution; x++)
            {
                heights[y, x] = flatHeight;
            }
        }

        terrainData.SetHeightsDelayLOD(0, 0, heights);
        terrain.Flush();
    }

    void PaintTexturesWithNoise()
    {
        if (terrainLayers == null || terrainLayers.Length == 0)
        {
            Debug.LogWarning("No terrain layers assigned!");
            return;
        }

        terrainData.terrainLayers = terrainLayers;

        int paintResolution = Mathf.Min(alphamapResolution, 512);
        float[,,] alphamap = new float[paintResolution, paintResolution, terrainLayers.Length];

        float offsetX = rng.Next(0, 10000);
        float offsetY = rng.Next(0, 10000);

        int grassCount = 0;
        int otherCount = 0;

        for (int y = 0; y < paintResolution; y++)
        {
            for (int x = 0; x < paintResolution; x++)
            {
                float normX = (float)x / paintResolution;
                float normY = (float)y / paintResolution;

                // Generate noise value (0-1) to determine if this is grass or other
                float noiseValue = Mathf.PerlinNoise(
                    normX * textureNoiseScale + offsetX,
                    normY * textureNoiseScale + offsetY
                );

                // 80% grass, 20% other layers - simple threshold
                if (noiseValue < grassPercentage)
                {
                    // This is grass (Layer 0)
                    alphamap[y, x, 0] = 1f;
                    grassCount++;
                }
                else
                {
                    // This is NOT grass - pick randomly from other layers
                    if (terrainLayers.Length > 1)
                    {
                        // Use a different noise to pick which layer
                        float layerNoise = Mathf.PerlinNoise(
                            normX * textureNoiseScale * 0.5f + offsetX + 500,
                            normY * textureNoiseScale * 0.5f + offsetY + 500
                        );

                        // Map noise to other layers (1, 2, 3, etc.)
                        int selectedLayer = 1 + Mathf.FloorToInt(layerNoise * (terrainLayers.Length - 1));
                        selectedLayer = Mathf.Clamp(selectedLayer, 1, terrainLayers.Length - 1);

                        alphamap[y, x, selectedLayer] = 1f;
                    }
                    else
                    {
                        // Only one layer available, use grass
                        alphamap[y, x, 0] = 1f;
                    }
                    otherCount++;
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, alphamap);

        float actualGrassPercentage = (float)grassCount / (paintResolution * paintResolution) * 100f;
        Debug.Log($"Textures painted: {actualGrassPercentage:F1}% grass, {100 - actualGrassPercentage:F1}% other layers");
    }

    void SpawnTrees()
    {
        if (treePrefabs == null || treePrefabs.Length == 0)
        {
            Debug.LogWarning("No tree prefabs assigned!");
            return;
        }

        if (treeParent == null) treeParent = new GameObject("Trees").transform;

        int spawnedCount = 0;

        for (int i = 0; i < treeCount; i++)
        {
            float x = (float)rng.NextDouble();
            float z = (float)rng.NextDouble();

            float worldX = x * terrainData.size.x + terrain.transform.position.x;
            float worldZ = z * terrainData.size.z + terrain.transform.position.z;

            float heightSample = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
            float worldY = heightSample + terrain.transform.position.y;

            // Check terrain layer at this position
            int alphaX = Mathf.FloorToInt(x * (alphamapResolution - 1));
            int alphaY = Mathf.FloorToInt(z * (alphamapResolution - 1));
            float[,,] alphamap = terrainData.GetAlphamaps(alphaX, alphaY, 1, 1);

            float layerWeight = alphamap[0, 0, treeSpawnLayer];

            if (layerWeight > treeLayerWeightThreshold)
            {
                Vector3 position = new Vector3(worldX, worldY, worldZ);
                GameObject treePrefab = treePrefabs[rng.Next(0, treePrefabs.Length)];
                Quaternion rotation = Quaternion.Euler(0, (float)rng.NextDouble() * 360f, 0);
                float randomScale = treeMinScale + (float)rng.NextDouble() * (treeMaxScale - treeMinScale);

                GameObject tree = Instantiate(treePrefab, position, rotation, treeParent);
                tree.transform.localScale = Vector3.one * randomScale;

                spawnedCount++;
            }
        }

        Debug.Log($"Spawned {spawnedCount} trees on layer {treeSpawnLayer}");
    }

    void SpawnGrass()
    {
        if (grassPrefabs == null || grassPrefabs.Length == 0)
        {
            Debug.LogWarning("No grass prefabs assigned!");
            return;
        }

        if (grassParent == null) grassParent = new GameObject("Grass").transform;

        int spawnedCount = 0;
        int attemptCount = 0;
        int maxAttempts = grassCount * 2;

        while (spawnedCount < grassCount && attemptCount < maxAttempts)
        {
            attemptCount++;

            float x = (float)rng.NextDouble();
            float z = (float)rng.NextDouble();

            float worldX = x * terrainData.size.x + terrain.transform.position.x;
            float worldZ = z * terrainData.size.z + terrain.transform.position.z;

            float heightSample = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
            float worldY = heightSample + terrain.transform.position.y;

            // Check terrain layer at this position
            int alphaX = Mathf.FloorToInt(x * (alphamapResolution - 1));
            int alphaY = Mathf.FloorToInt(z * (alphamapResolution - 1));
            float[,,] alphamap = terrainData.GetAlphamaps(alphaX, alphaY, 1, 1);

            float layerWeight = alphamap[0, 0, grassSpawnLayer];

            if (layerWeight > grassLayerWeightThreshold && (float)rng.NextDouble() < grassDensity)
            {
                Vector3 position = new Vector3(worldX, worldY, worldZ);
                GameObject grassPrefab = grassPrefabs[rng.Next(0, grassPrefabs.Length)];
                Quaternion rotation = Quaternion.Euler(0, (float)rng.NextDouble() * 360f, 0);
                float scale = grassMinScale + (float)rng.NextDouble() * (grassMaxScale - grassMinScale);

                GameObject grass = Instantiate(grassPrefab, position, rotation, grassParent);
                grass.transform.localScale = Vector3.one * scale;
                spawnedCount++;
            }
        }

        Debug.Log($"Spawned {spawnedCount} grass objects on layer {grassSpawnLayer}");
    }

    void SpawnDecorations()
    {
        if (decorationPrefabs == null || decorationPrefabs.Length == 0) return;

        if (decorationParent == null) decorationParent = new GameObject("Decorations").transform;

        int spawnedCount = 0;

        for (int i = 0; i < decorationCount; i++)
        {
            float x = (float)rng.NextDouble();
            float z = (float)rng.NextDouble();

            float worldX = x * terrainData.size.x + terrain.transform.position.x;
            float worldZ = z * terrainData.size.z + terrain.transform.position.z;

            float heightSample = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
            float worldY = heightSample + terrain.transform.position.y;

            // Check terrain layer at this position
            int alphaX = Mathf.FloorToInt(x * (alphamapResolution - 1));
            int alphaY = Mathf.FloorToInt(z * (alphamapResolution - 1));
            float[,,] alphamap = terrainData.GetAlphamaps(alphaX, alphaY, 1, 1);

            bool shouldSpawn = false;

            if (decorationSpawnLayer < 0)
            {
                // Spawn on any layer
                shouldSpawn = true;
            }
            else if (decorationSpawnLayer < terrainLayers.Length)
            {
                float layerWeight = alphamap[0, 0, decorationSpawnLayer];
                shouldSpawn = layerWeight > decorationLayerWeightThreshold;
            }

            if (shouldSpawn)
            {
                Vector3 position = new Vector3(worldX, worldY, worldZ);
                GameObject decorationPrefab = decorationPrefabs[rng.Next(0, decorationPrefabs.Length)];
                Quaternion rotation = Quaternion.Euler(0, (float)rng.NextDouble() * 360f, 0);
                float randomScale = decorationMinScale + (float)rng.NextDouble() * (decorationMaxScale - decorationMinScale);

                GameObject decoration = Instantiate(decorationPrefab, position, rotation, decorationParent);
                decoration.transform.localScale = Vector3.one * randomScale;

                spawnedCount++;
            }
        }

        string layerInfo = decorationSpawnLayer < 0 ? "any layer" : $"layer {decorationSpawnLayer}";
        Debug.Log($"Spawned {spawnedCount} decorations on {layerInfo}");
    }

    [ContextMenu("Regenerate Terrain")]
    public void RegenerateTerrain()
    {
        CleanupAllGenerated();

        seed = Random.Range(0, 100000);
        rng = new System.Random(seed);
        GenerateTerrain();
        Debug.Log($"Terrain Regenerated with Seed: {seed}");
    }

    [ContextMenu("Clear All Generated Content")]
    public void ClearAllContent()
    {
        CleanupAllGenerated();
        Debug.Log("All generated content cleared!");
    }

    void OnDestroy()
    {
        Debug.Log("Terrain Generator destroyed - cleanup complete");
    }
}