using UnityEngine;

public class ProceduralTerrainGenerator : MonoBehaviour
{
    [Header("Terrain Reference")]
    public Terrain terrain;

    [Header("Height Generation")]
    [Range(0.001f, 0.1f)]
    public float heightScale = 0.02f; // Small values for subtle variation
    [Range(1f, 50f)]
    public float noiseScale = 20f;
    public int seed = 0; // Change for different terrains

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
    public float grassDensity = 0.7f; // Probability of spawning at valid location
    public Transform grassParent;

    [Header("Obstacles/Decorations (Rocks, Bushes, etc.)")]
    public GameObject[] obstaclePrefabs;
    [Range(0, 2000)]
    public int obstacleCount = 500;
    [Range(0f, 1f)]
    public float obstacleHeightMin = 0.1f;
    [Range(0f, 1f)]
    public float obstacleHeightMax = 0.7f;

    // --- ADDED SCALE VARIABLES ---
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

        // 1. Create a temporary copy of the TerrainData so we don't corrupt the asset file
        terrain.terrainData = Instantiate(terrain.terrainData);

        // 2. IMPORTANT: You must also update the Collider, or you will fall through the floor!
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

                // Perlin noise gives values 0-1, multiply by small heightScale for subtle variation
                heights[y, x] = Mathf.PerlinNoise(xCoord, yCoord) * heightScale;
            }
        }

        // Important: Use SetHeightsDelayLOD for better performance and stability
        terrainData.SetHeightsDelayLOD(0, 0, heights);
        terrain.Flush(); // Apply changes
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

                // 1. Get the raw height (0.0 to 1.0 relative to total terrain height)
                float rawHeight = terrainData.GetHeight(sampleY, sampleX) / terrainSize.y;

                // 2. Calculate "Relative Height"
                float height = Mathf.Clamp01(rawHeight / heightScale);

                // Calculate slope
                float slope = 1f - terrainData.GetInterpolatedNormal(normX, normY).y;

                float noiseValue = Mathf.PerlinNoise(
                    normX * textureNoiseScale + offsetX,
                    normY * textureNoiseScale + offsetY
                );

                float[] weights = new float[terrainLayers.Length];

                // Layer 0: Grass (Using your Slider)
                if (terrainLayers.Length > 0)
                {
                    float heightWeight = (height > grassHeightThreshold) ? 1.0f : 0f;
                    weights[0] = heightWeight * (1f - slope);
                    weights[0] *= (0.5f + noiseValue * 0.5f);
                }

                // Layer 1: Rock/Dirt (Slope based OR very high up)
                if (terrainLayers.Length > 1)
                {
                    float slopeWeight = slope > rockSlopeThreshold ? slope : 0f;
                    weights[1] = slopeWeight + (height > 0.7f ? height : 0f);
                }

                // Layer 2: Sand/Path (Low areas)
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
        // 1. Remove the Mesh Collider (Terrain doesn't support it for trees)
        MeshCollider existingMeshCollider = prefab.GetComponent<MeshCollider>();
        if (existingMeshCollider != null)
        {
            DestroyImmediate(existingMeshCollider, true);
        }

        // 2. Check if it already has a Capsule Collider
        CapsuleCollider capsule = prefab.GetComponent<CapsuleCollider>();
        if (capsule == null)
        {
            capsule = prefab.AddComponent<CapsuleCollider>();
        }

        // 3. Auto-size the capsule based on the mesh bounds
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

    // --- FIX START: UPDATED SPAWN GRASS (Positions Fixed) ---
    void SpawnGrass()
    {
        if (grassPrefabs == null || grassPrefabs.Length == 0)
        {
            Debug.LogWarning("No grass prefabs assigned!");
            return;
        }

        if (grassParent == null) grassParent = new GameObject("Grass").transform;
        while (grassParent.childCount > 0) DestroyImmediate(grassParent.GetChild(0).gameObject);

        int spawnedCount = 0;
        int attemptCount = 0;
        int maxAttempts = grassCount * 2;

        while (spawnedCount < grassCount && attemptCount < maxAttempts)
        {
            attemptCount++;

            float x = (float)rng.NextDouble();
            float z = (float)rng.NextDouble();

            // 1. Calculate Absolute World Coordinates used for sampling
            float worldX = x * terrainData.size.x + terrain.transform.position.x;
            float worldZ = z * terrainData.size.z + terrain.transform.position.z;

            // 2. Sample Height returns Y relative to terrain pivot, so we add terrain Y back
            float heightSample = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
            float worldY = heightSample + terrain.transform.position.y;

            // Normalize height for the slider
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
                // 3. Create Vector with calculated WorldY
                Vector3 position = new Vector3(worldX, worldY, worldZ);

                GameObject grassPrefab = grassPrefabs[rng.Next(0, grassPrefabs.Length)];

                Quaternion rotation = Quaternion.Euler(0, (float)rng.NextDouble() * 360f, 0);
                float scale = grassMinScale + (float)rng.NextDouble() * (grassMaxScale - grassMinScale);

                GameObject grass = Instantiate(grassPrefab, position, rotation, grassParent);
                grass.transform.localScale = Vector3.one * scale;
                spawnedCount++;
            }
        }

        Debug.Log($"=== GRASS SPAWNING COMPLETE. Spawned: {spawnedCount} ===");
    }
    // --- FIX END: GRASS ---

    // --- FIX START: UPDATED OBSTACLES (Positions Fixed + Scaling Added) ---
    void SpawnObstacles()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        if (obstacleParent == null) obstacleParent = new GameObject("Obstacles").transform;
        while (obstacleParent.childCount > 0) DestroyImmediate(obstacleParent.GetChild(0).gameObject);

        int spawnedCount = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            float x = (float)rng.NextDouble();
            float z = (float)rng.NextDouble();

            // 1. Calculate Absolute World Coordinates
            float worldX = x * terrainData.size.x + terrain.transform.position.x;
            float worldZ = z * terrainData.size.z + terrain.transform.position.z;

            // 2. Sample Height + Terrain Y
            float heightSample = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));
            float worldY = heightSample + terrain.transform.position.y;

            // Normalize height for the slider check
            float normalizedHeight = Mathf.Clamp01((heightSample / terrainData.size.y) / heightScale);

            float[,,] alphamap = terrainData.GetAlphamaps(Mathf.FloorToInt(x * (alphamapResolution - 1)), Mathf.FloorToInt(z * (alphamapResolution - 1)), 1, 1);
            float grassWeight = alphamap[0, 0, 0];

            if (normalizedHeight >= obstacleHeightMin && normalizedHeight <= obstacleHeightMax && grassWeight > 0.5f)
            {
                // 3. Use calculated WorldY
                Vector3 position = new Vector3(worldX, worldY, worldZ);

                GameObject obstaclePrefab = obstaclePrefabs[rng.Next(0, obstaclePrefabs.Length)];

                Quaternion rotation = Quaternion.Euler(0, (float)rng.NextDouble() * 360f, 0);

                // 4. New Scale Logic using Min/Max
                float randomScale = obstacleMinScale + (float)rng.NextDouble() * (obstacleMaxScale - obstacleMinScale);

                GameObject obstacle = Instantiate(obstaclePrefab, position, rotation, obstacleParent);
                obstacle.transform.localScale = Vector3.one * randomScale;

                spawnedCount++;
            }
        }
        Debug.Log($"Spawned {spawnedCount} obstacles with scale range {obstacleMinScale}-{obstacleMaxScale}");
    }
    // --- FIX END: OBSTACLES ---

    [ContextMenu("Regenerate Terrain")]
    public void RegenerateTerrain()
    {
        seed = rng.Next(0, 100000);
        rng = new System.Random(seed);
        GenerateTerrain();
        Debug.Log($"Terrain Regenerated with Seed: {seed}");
    }
}