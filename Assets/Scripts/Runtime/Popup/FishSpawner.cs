using UnityEngine;

public class FishingSpawner : MonoBehaviour
{
    [Header("Spawn Rows")]
    public RectTransform fishRow;
    public RectTransform trashRow;

    [Header("Pool Tags")]
    public string fishPoolTag = "Fish";
    public string trashPoolTag = "Trash";

    [Header("Count")]
    public int fishCountRange;
    public int trashCountRange;
    private void Start()
    {
        fishCountRange = Random.Range(3, 5);
        trashCountRange = Random.Range(2, 4);
        SpawnWave();
        Debug.Log("Spawned a new wave of fish and trash!");
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
        float startX = -(spacing * (fishCountRange - 1)) / 2f;

        for (int i = 0; i < fishCountRange; i++)
        {
            GameObject go = PoolManager.Instance.SpawnFromPool(fishPoolTag, fishRow);
            go.GetComponent<FishItem>().Init();

            RectTransform rt = go.GetComponent<RectTransform>();

            float x = startX + spacing * i;
            float y = Mathf.Sin(i * 0.8f) * waveHeight;

            rt.anchoredPosition = new Vector2(x, y);
        }
    }
    void SpawnTrash()
    {
        float spacing = 320f;
        float startX = -(spacing * (trashCountRange - 1)) / 2f;

        for (int i = 0; i < trashCountRange; i++)
        {
            GameObject go = PoolManager.Instance.SpawnFromPool(trashPoolTag, trashRow);

            RectTransform rt = go.GetComponent<RectTransform>();

            float x = startX + spacing * i;
            rt.anchoredPosition = new Vector2(x, 0f);
        }
    }
}