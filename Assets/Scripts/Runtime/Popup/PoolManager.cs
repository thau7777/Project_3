using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pool
{
    public string tag;           // Tên định danh pool
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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    // ================= SPAWN =================
    public GameObject SpawnFromPool(string tag, Transform parent)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError("❌ Pool tag không tồn tại: " + tag);
            return null;
        }

        if (poolDictionary[tag].Count == 0)
        {
            ExpandPool(tag);
        }

        GameObject obj = poolDictionary[tag].Dequeue();

        obj.transform.SetParent(parent);
        obj.SetActive(true);

        return obj;
    }

    // ================= DESPAWN =================
    public void Despawn(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogError("❌ Pool tag không tồn tại: " + tag);
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(transform);
        poolDictionary[tag].Enqueue(obj);
    }

    // ================= EXPAND =================
    void ExpandPool(string tag)
    {
        Pool pool = pools.Find(p => p.tag == tag);
        if (pool == null)
        {
            Debug.LogError("❌ Không tìm thấy pool để expand: " + tag);
            return;
        }

        GameObject obj = Instantiate(pool.prefab, transform);
        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);

        Debug.LogWarning("⚠ Pool '" + tag + "' bị thiếu – đã tạo thêm 1 object");
    }
}