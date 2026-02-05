using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string tag;           // Tên định danh pool (VD: "MoleNormal", "MoleGold")
    public GameObject prefab;    // Prefab cần pool
    public int size;             // Số lượng khởi tạo ban đầu
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false); // Ẩn đi khi mới tạo
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Transform parent)
    {
        if (!poolDictionary.ContainsKey(tag)) return null;

        if (poolDictionary[tag].Count == 0)
        {
            // Tìm prefab tương ứng trong list pools để tạo thêm
            Pool poolToExpand = pools.Find(p => p.tag == tag);
            if (poolToExpand != null)
            {
                GameObject obj = Instantiate(poolToExpand.prefab);
                obj.SetActive(false);
                poolDictionary[tag].Enqueue(obj);
            }
            else
            {
                Debug.LogWarning("Không tìm thấy pool để mở rộng với tag: " + tag);
                return null;
            }
            // Hoặc đơn giản là thông báo lỗi để bạn tăng "size" trong Inspector
            Debug.LogWarning("Pool " + tag + " hết hàng rồi!");
        }
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetParent(parent);

        // Đưa lại vào cuối hàng đợi để tái sử dụng
        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }
}