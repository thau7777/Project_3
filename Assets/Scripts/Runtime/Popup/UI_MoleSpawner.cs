using UnityEditor.EditorTools;
using UnityEngine;

public class UI_MoleSpawner : MonoBehaviour
{
    public string moleTag = "Mole"; // Tag đặt trong PoolManager
    public RectTransform[] spawnPoints;
    public float spawnDelay = 1.5f;
    public float randomOffset = 30f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnMole), 1f, spawnDelay);
    }

    void SpawnMole()
    {
        int index = Random.Range(0, spawnPoints.Length);
        RectTransform point = spawnPoints[index];

        // Lấy mole từ pool thay vì Instantiate
        GameObject mole = PoolManager.Instance.SpawnFromPool(moleTag, point.parent);

        if (mole != null)
        {
            RectTransform moleRect = mole.GetComponent<RectTransform>();

            Vector2 offset = new Vector2(
                Random.Range(-randomOffset, randomOffset),
                Random.Range(-randomOffset, randomOffset)
            );

            moleRect.anchoredPosition = point.anchoredPosition + offset;
        }
    }
}