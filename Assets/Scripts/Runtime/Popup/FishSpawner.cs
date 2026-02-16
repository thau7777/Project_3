using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public FishingDatabase database;
    public Vector2 minPos;
    public Vector2 maxPos;

    public FishItem Spawn()
    {
        FishingItemData data = database.GetRandomItem();

        Vector3 pos = new Vector3(
            Random.Range(minPos.x, maxPos.x),
            Random.Range(minPos.y, maxPos.y),
            0f
        );

        GameObject obj = Instantiate(data.prefab, pos, Quaternion.identity);
        FishItem item = obj.GetComponent<FishItem>();
        item.data = data;

        return item;
    }
}