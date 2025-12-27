using UnityEngine;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    [Header("Terrain Reference")]
    public Terrain terrain;

    [Header("Height Generation")]
    [Range(0.001f, 0.1f)]
    public float heightScale = 0.02f;
    [Range(1f, 50f)]
    public float noiseScale = 20f;
    public int seed = 0;

    [Header("Texture Layers (Assign in order)")]
    [Tooltip("0: Grass, 1: Dirt/Rock, 2: Sand/Path, etc.")]
    public TerrainLayer[] terrainLayers;

    [Header("Texture Painting Settings")]
    [Range(0f, 1f)]
    public float grassHeightThreshold = 0.3f;
    [Range(0f, 1f)]
    public float rockSlopeThreshold = 0.5f;
    public float textureNoiseScale = 15f;

    [Header("Trees")]
    public GameObject[] treePrefabs;
    [Range(0, 1000)]
    public int treeCount = 100;
    [Range(0f, 1f)]
    public float treeHeightMin = 0.2f;
    [Range(0f, 1f)]
    public float treeHeightMax = 0.8f;

    [Header("Grass (Instantiated GameObjects)")]
    public GameObject[] grassPrefabs;
    [Range(0, 10000)]
    public int grassCount = 2000;
    [Range(0f, 1f)]
    public float grassHeightMin = 0.1f;
    [Range(0f, 1f)]
    public float grassHeightMax = 0.7f;
    [Range(0.1f, 5f)]
    public float grassMinScale = 0.5f;
    [Range(0.1f, 5f)]
    public float grassMaxScale = 1.5f;
    [Range(0f, 1f)]
    public float grassDensity = 0.7f;
    public Transform grassParent;

    [Header("Obstacles/Decorations (Rocks, Bushes, etc.)")]
    public GameObject[] obstaclePrefabs;
    [Range(0, 2000)]
    public int obstacleCount = 500;
    [Range(0f, 1f)]
    public float obstacleHeightMin = 0.1f;
    [Range(0f, 1f)]
    public float obstacleHeightMax = 0.7f;
    [Range(0.1f, 10f)]
    public float obstacleMinScale = 0.5f;
    [Range(0.1f, 10f)]
    public float obstacleMaxScale = 2.0f;
    public Transform obstacleParent;

    private TerrainData terrainData;
    private int heightmapResolution;
    private int alphamapResolution;
    private int detailResolution;
    private System.Random rng;

    void Start()
    {
        Debug.Log("=== Terrain Generator Starting ===");

        if (terrain == null)
            terrain = GetComponent<Terrain>();

        // CLEANUP FIRST - Remove all old generated content
        CleanupAllGenerated();

        // Create a temporary copy of the TerrainData
        terrain.terrainData = Instantiate(terrain.terrainData);

        // Update the Collider
        if (terrain.GetComponent<TerrainCollider>())
        {
            terrain.GetComponent<TerrainCollider>().terrainData = terrain.terrainData;
        }

        terrainData = terrain.terrainData;
        if (terrainData == null)
        {
            Debug.LogError("Terrain has no TerrainData!");
            return;
        }

        heightmapResolution = terrainData.heightmapResolution;
        alphamapResolution = terrainData.alphamapResolution;
        detailResolution = terrainData.detailResolution;

        Debug.Log($"Heightmap Resolution: {heightmapResolution}");
        Debug.Log($"Alphamap Resolution: {alphamapResolution}");
        Debug.Log($"Detail Resolution: {detailResolution}");

        rng = new System.Random(seed);

        GenerateTerrain();
    }

    // === CLEANUP METHOD - Removes all old generated content ===
    void CleanupAllGenerated()
    {
        Debug.Log("Cleaning up old generated content...");

        // Clean up grass
        if (grassParent != null)
        {
            int grassChildCount = grassParent.childCount;
            while (grassParent.childCount > 0)
            {
                DestroyImmediate(grassParent.GetChild(0).gameObject);
            }
            Debug.Log($"Removed {grassChildCount} old grass objects");
        }

        // Clean up obstacles
        if (obstacleParent != null)
        {
            int obstacleChildCount = obstacleParent.childCount;
            while (obstacleParent.childCount > 0)
            {
                DestroyImmediate(obstacleParent.GetChild(0).gameObject);
            }
            Debug.Log($"Removed {obstacleChildCount} old obstacle objects");
        }

        // Clean up trees from terrain
        if (terrain != null && terrain.terrainData != null)
        {
            int oldTreeCount = terrain.terrainData.treeInstances.Length;
            terrain.terrainData.treeInstances = new TreeInstance[0];
            if (oldTreeCount > 0)
                Debug.Log($"Removed {oldTreeCount} old trees");
        }

        // Force garbage collection (optional but helpful)
        System.GC.Collect();
        Resources.UnloadUnusedAssets();

        Debug.Log("Cleanup complete!");
    }

    void GenerateTerrain()
    {
        Debug.Log("Step 1: Generating heights...");
        GenerateHeights();
        Debug.Log("Heights complete!");

        Debug.Log("Step 2: Painting textures...");
        PaintTextures();
        Debug.Log("Textures complete!");

        Debug.Log("Step 3: Placing trees...");
        PlaceTrees();
        Debug.Log("Trees complete!");

        Debug.Log("Step 4: Spawning grass...");
        SpawnGrass();
        Debug.Log("Grass complete!");

        Debug.Log("Step 5: Spawning obstacles/decorations...");
        SpawnObstacles();
        Debug.Log("Obstacles complete!");

        Debug.Log("=== Terrain Generation Complete ===");
    }

    void GenerateHeights()
    {
        float[,] heights = new float[heightmapResolution, heightmapResolution];
        float offsetX = rng.Next(0, 10000);
        float offsetY = rng.Next(0, 10000);

        for (int y = 0; y < heightmapResolution; y++)
        {
            for (int x = 0; x < heightmapResolution; x++)
            {
                float xCoord = (float)x / heightmapResolution * noiseScale + offsetX;
                float yCoord = (float)y / heightmapResolution * noiseScale + offsetY;

                heights[y, x] = Mathf.PerlinNoise(xCoord, yCoord) * heightScale;
            }
        }

        terrainData.SetHeightsDelayLOD(0, 0, heights);
        terrain.Flush();
    }

    void PaintTextures()
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

        Vector3 terrainSize = terrainData.size;
        int heightmapRes = terrainData.heightmapResolution;

        for (int y = 0; y < paintResolution; y++)
        {
            for (int x = 0; x < paintResolution; x++)
            {
                float normX = (float)x / paintResolution;
                float normY = (float)y / paintResolution;

                int sampleX = Mathf.RoundToInt(normX * (heightmapRes - 1));
                int sampleY = Mathf.RoundToInt(normY * (heightmapRes - 1));

                float rawHeight = terrainData.GetHeight(sampleY, sampleX) / terrainSize.y;
                float height = Mathf.Clamp01(rawHeight / heightScale);
                float slope = 1f - terrainData.GetInterpolatedNormal(normX, normY).y;

                float noiseValue = Mathf.PerlinNoise(
                    normX * textureNoiseScale + offsetX,
                    normY * textureNoiseScale + offsetY
                );

                float[] weights = new float[terrainLayers.Length];

                // Layer 0: Grass
                if (terrainLayers.Length > 0)
                {
                    float heightWeight = (height > grassHeightThreshold) ? 1.0f : 0f;
                    weights[0] = heightWeight * (1f - slope);
                    weights[0] *= (0.5f + noiseValue * 0.5f);
                }

                // Layer 1: Rock/Dirt
                if (terrainLayers.Length > 1)
                {
                    float slopeWeight = slope > rockSlopeThreshold ? slope : 0f;
                    weights[1] = slopeWeight + (height > 0.7f ? height : 0f);
                }

                // Layer 2: Sand/Path
                if (terrainLayers.Length > 2)
                {
                    weights[2] = (height < 0.3f ? (1f - height) : 0f) * noiseValue;
                }

                // Normalize weights
                float totalWeight = 0f;
                for (int i = 0; i < weights.Length; i++) totalWeight += weights[i];

                if (totalWeight > 0.001f)
                {
                    for (int i = 0; i < weights.Length; i++)
                        alphamap[y, x, i] = weights[i] / totalWeight;
                }
                else
                {
                    alphamap[y, x, 0] = 1f;
                }
            }
        }

        terrainData.SetAlphamaps(0, 0, alphamap);
    }

    private void SetupTreeColliders(GameObject prefab)
    {
        MeshCollider existingMeshCollider = prefab.GetComponent<MeshCollider>();
        if (existingMeshCollider != null)
        {
            DestroyImmediate(existingMeshCollider, true);
        }

        CapsuleCollider capsule = prefab.GetComponent<CapsuleCollider>();
        if (capsule == null)
        {
            capsule = prefab.AddComponent<CapsuleCollider>();
        }

        MeshRenderer meshRenderer = prefab.GetComponentInChildren<MeshRenderer>();
        if (meshRenderer != null)
        {
            Bounds bounds = meshRenderer.bounds;
            capsule.height = bounds.size.y;
            capsule.radius = (bounds.size.x + bounds.size.z) / 4f * 0.15f;
            capsule.center = new Vector3(0, capsule.height / 2f, 0);
        }
    }

    void PlaceTrees()
    {
        if (treePrefabs == null || treePrefabs.Length == 0) return;

        terrainData.treeInstances = new TreeInstance[0];
        TreePrototype[] treePrototypes = new TreePrototype[treePrefabs.Length];

        for (int i = 0; i < treePrefabs.Length; i++)
        {
            if (treePrefabs[i] != null)
            {
                SetupTreeColliders(treePrefabs[i]);
                treePrototypes[i] = new TreePrototype { prefab = treePrefabs[i] };
            }
        }

        terrainData.treePrototypes = treePrototypes;

        TreeInstance[] trees = new TreeInstance[treeCount];
        int treeIndex = 0;

        for (int i = 0; i < treeCount; i++)
        {
            float x = (float)rng.NextDouble();
            float z = (float)rng.NextDouble();

            float rawHeight = terrainData.GetInterpolatedHeight(x, z) / terrainData.size.y;
            float normalizedHeight = Mathf.Clamp01(rawHeight / heightScale);

            float[,,] alphamap = terrainData.GetAlphamaps(Mathf.FloorToInt(x * (alphamapResolution - 1)), Mathf.FloorToInt(z * (alphamapResolution - 1)), 1, 1);
            float grassWeight = alphamap[0, 0, 0];

            if (normalizedHeight >= treeHeightMin && normalizedHeight <= treeHeightMax && grassWeight > 0.5f)
            {
                TreeInstance tree = new TreeInstance();
                tree.position = new Vector3(x, rawHeight, z);
                tree.prototypeIndex = rng.Next(0, treePrefabs.Length);
                tree.widthScale = 0.8f + (float)rng.NextDouble() * 0.4f;
                tree.heightScale = 0.8f + (float)rng.NextDouble() * 0.4f;
                tree.color = Color.white;
                tree.lightmapColor = Color.white;

                trees[treeIndex] = tree;
                treeIndex++;
            }
        }

        if (treeIndex > 0)
        {
            System.Array.Resize(ref trees, treeIndex);
            terrainData.treeInstances = trees;
        }
        terrainData.RefreshPrototypes();

        TerrainCollider tc = terrain.GetComponent<TerrainCollider>();
        if (tc != null)
        {
            tc.enabled = false;
            tc.enabled = true;
        }
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

            float normalizedHeight = Mathf.Clamp01((heightSample / terrainData.size.y) / heightScale);

            int alphaX = Mathf.FloorToInt(x * (alphamapResolution - 1));
            int alphaY = Mathf.FloorToInt(z * (alphamapResolution - 1));
            float[,,] alphamap = terrainData.GetAlphamaps(alphaX, alphaY, 1, 1);
            float grassWeight = alphamap[0, 0, 0];

            if (normalizedHeight >= grassHeightMin &&
                normalizedHeight <= grassHeightMax &&
                grassWeight > 0.3f &&
                (float)rng.NextDouble() < grassDensity)
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

        Debug.Log($"Spawned {spawnedCount} grass objects");
    }

    void SpawnObstacles()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        if (obstacleParent == null) obstacleParent = new GameObject("Obstacles").transform;

        int spawnedCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            float x = (float)rng.NextDouble();
            float z = (float)rng.NextDouble();

            float worldX = x * terrainData.size.x + terrain.transform.position.x;
            float worldZ = z * terrainData.size.z + terrain.transform.position.z;

            float heightSample = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
            float worldY = heightSample + terrain.transform.position.y;

            float normalizedHeight = Mathf.Clamp01((heightSample / terrainData.size.y) / heightScale);

            float[,,] alphamap = terrainData.GetAlphamaps(Mathf.FloorToInt(x * (alphamapResolution - 1)), Mathf.FloorToInt(z * (alphamapResolution - 1)), 1, 1);
            float grassWeight = alphamap[0, 0, 0];

            if (normalizedHeight >= obstacleHeightMin && normalizedHeight <= obstacleHeightMax && grassWeight > 0.5f)
            {
                Vector3 position = new Vector3(worldX, worldY, worldZ);

                GameObject obstaclePrefab = obstaclePrefabs[rng.Next(0, obstaclePrefabs.Length)];

                Quaternion rotation = Quaternion.Euler(0, (float)rng.NextDouble() * 360f, 0);

                float randomScale = obstacleMinScale + (float)rng.NextDouble() * (obstacleMaxScale - obstacleMinScale);

                GameObject obstacle = Instantiate(obstaclePrefab, position, rotation, obstacleParent);
                obstacle.transform.localScale = Vector3.one * randomScale;

                spawnedCount++;
            }
        }
        Debug.Log($"Spawned {spawnedCount} obstacles");
    }

    [ContextMenu("Regenerate Terrain")]
    public void RegenerateTerrain()
    {
        CleanupAllGenerated(); // Clean before regenerating

        seed = rng.Next(0, 100000);
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

    // Clean up when script is disabled/destroyed
    void OnDestroy()
    {
        Debug.Log("Terrain Generator destroyed - performing final cleanup");
        // Note: We don't destroy the parent objects here as they might be needed
    }
}