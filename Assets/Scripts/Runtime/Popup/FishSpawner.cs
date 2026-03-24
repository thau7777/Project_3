using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingSpawner : MonoBehaviour
{
    [Header("Spawn Rows")]
    public RectTransform fishRow;
    public RectTransform trashRow;

    [Header("Fish Prefabs")]
    public List<GameObject> fishPrefabs;
    public int fishCount;

    [Header("Trash Prefabs")]
    public List<GameObject> trashPrefabs;
    public Vector2Int trashCountRange = new Vector2Int(1, 4);

    [Header("Treasure")]
    public GameObject treasurePrefab;
    [Range(0f, 1f)] public float treasureChance = 0.35f;

    public static FishingSpawner instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        
    }
    private void Start()
    {
        fishCount = Random.Range(3,5);
        SpawnWave();
    }

    public void SpawnWave()
    {
        SpawnFish();
        SpawnTrash();
    }

    void SpawnFish()
    {
        float spacing = 280f;
        float waveHeight = 100f;
        float startX = -(spacing * (fishCount - 1)) / 2f;

        for (int i = 0; i < fishCount; i++)
        {
            GameObject prefab = GetRandomFishPrefab();
            GameObject go = Instantiate(prefab, fishRow);

            go.GetComponent<FishItem>()?.Init();

            RectTransform rt = go.GetComponent<RectTransform>();

            float x = startX + spacing * i;
            float y = Mathf.Sin(i * 0.8f) * waveHeight;

            rt.anchoredPosition = new Vector2(x, Random.Range(-y,y));
        }

        // 🎁 spawn rương kho báu theo tỉ lệ
        if (treasurePrefab != null && Random.value < treasureChance)
        {
            SpawnTreasure();
        }
    }

    GameObject GetRandomFishPrefab()
    {
        return fishPrefabs[Random.Range(0, fishPrefabs.Count)];
    }

    void SpawnTrash()
    {
        int trashCount = Random.Range(trashCountRange.x, trashCountRange.y + 1);

        float spacing = 320f;
        float startX = -(spacing * (trashCount - 1)) / 2f;

        for (int i = 0; i < trashCount; i++)
        {
            GameObject prefab = trashPrefabs[Random.Range(0, trashPrefabs.Count)];
            GameObject go = Instantiate(prefab, trashRow);

            RectTransform rt = go.GetComponent<RectTransform>();
            float x = startX + spacing * i;

            rt.anchoredPosition = new Vector2(x, 0f);
        }
    }

    void SpawnTreasure()
    {
        RectTransform treasueRow = Random.value < 0.5f ? fishRow : trashRow;

        GameObject go = Instantiate(treasurePrefab, treasueRow);
        RectTransform rt = go.GetComponent<RectTransform>();

        rt.anchoredPosition = new Vector2(
            Random.Range(-300f, 200f),
            Random.Range(-120f,50f)
        );
    }
}