using UnityEngine;
using System.Collections.Generic;

public enum TerrainLayer
{
    GrassOnly,   // Only spawn where noise > grassThreshold (grass texture)
    DirtOnly,    // Only spawn where noise < grassThreshold (dirt texture)
    Both         // Spawn on both grass and dirt textures
}

[System.Serializable]
public class EnviromentSpawnData
{
    public List<GameObject> prefabs;

    [LabelText("Spawn Count", LabelTextAttribute.LabelColor.yellow)]
    public int count = 10;

    [TabGroup("Placement")]
    [Tooltip("GrassOnly = spawns only on grass texture areas\nDirtOnly = spawns only on dirt texture areas\nBoth = spawns on both textures")]
    public TerrainLayer terrainLayer = TerrainLayer.GrassOnly;

    [TabGroup("Placement")]
    [Tooltip("Checks surrounding area within this radius to ensure it's all the same terrain type. Set to 0 to disable. Recommended: 0.5-2.0")]
    [Range(0f, 5f)]
    public float edgeSafetyRadius = 1f;

    [TabGroup("Placement")]
    [Tooltip("Number of points to check around the spawn position. Higher = more accurate but slower. Recommended: 4-8")]
    [Range(4, 16)]
    public int edgeCheckSamples = 8;

    [TabGroup("Transform")]
    [MinMaxSlider(0.1f, 5f)]
    [LabelText("Test",LabelTextAttribute.LabelColor.orange)]
    public Vector2 scaleRange = new Vector2(0.8f, 1.2f);

    [TabGroup("Transform")]
    [MinMaxSlider(0, 360)]
    public Vector2 rotationYRange = new Vector2(0, 360);
}