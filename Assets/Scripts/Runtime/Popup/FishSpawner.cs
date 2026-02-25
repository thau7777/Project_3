using UnityEngine;

public class FishingSpawner : MonoBehaviour
{
    [Header("Spawn Rows")]
    public Transform[] spawnRows;

    [Header("Pool Tags")]
    public string fishPoolTag = "Fish";
    public string trashPoolTag = "Trash";

    [Header("Count")]
    public Vector2Int fishCountRange = new(3, 4);
    public Vector2Int trashCountRange = new(2, 3);

    private void Start()
    {
        SpawnWave();
        Debug.Log("Spawned a new wave of fish and trash!");
    }
    public void SpawnWave()
    {
        Spawn(fishPoolTag, fishCountRange);
        Spawn(trashPoolTag, trashCountRange);
    }

    void Spawn(string tag, Vector2Int range)
    {
        int count = Random.Range(range.x, range.y + 1);

        for (int i = 0; i < count; i++)
        {
            Transform row = spawnRows[Random.Range(0, spawnRows.Length)];
            GameObject go = PoolManager.Instance.SpawnFromPool(tag, row);

            FishItem item = go.GetComponent<FishItem>();
            item.Init();

            Vector3 pos = go.transform.localPosition;
            pos.x = Random.Range(-6f, 6f);
            go.transform.localPosition = pos;
        }
    }
}