using System.Collections.Generic;
using UnityEngine;

public class UI_MoleSpawner : MonoBehaviour
{
    public string moleTag = "Mole";
    public GameObject holePrefab; // Kéo Prefab cái hang vào đây
    public RectTransform container;
    public int spawnPointCount = 6;
    public float radius = 250f;
    public float spawnDelay = 1.5f;

    // Lưu danh sách các điểm Slot bên trong mỗi cái hang
    private List<Transform> _holeSlots = new List<Transform>();
    private List<bool> _isOccupied = new List<bool>();

    void OnEnable()
    {
        GenerateHoles();
        InvokeRepeating(nameof(SpawnMole), 0.5f, spawnDelay);
    }

    void OnDisable()
    {
        CancelInvoke(nameof(SpawnMole));
        // Xóa các hang cũ khi ẩn UI
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    void GenerateHoles()
    {
        _holeSlots.Clear();
        _isOccupied.Clear();

        float startAngleOffset = Random.Range(0f, Mathf.PI * 2);

        for (int i = 0; i < spawnPointCount; i++)
        {
            // Tính góc và vị trí (thêm một chút random nhẹ để không quá đều)
            float angle = (i * Mathf.PI * 2 / spawnPointCount) + startAngleOffset;
            float x = Mathf.Cos(angle) * (radius + Random.Range(-20f, 20f));
            float y = Mathf.Sin(angle) * (radius + Random.Range(-20f, 20f));

            // Tạo hang
            GameObject hole = Instantiate(holePrefab, container);
            hole.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

            // Tìm Object con tên là "Mole_Slot" để sau này nhét Mole vào đó
            Transform slot = hole.transform.Find("Mole_Slot");
            _holeSlots.Add(slot != null ? slot : hole.transform);
            _isOccupied.Add(false);
        }
    }

    void SpawnMole()
    {
        // Lọc danh sách các chỉ số (index) đang trống
        List<int> freeIndices = new List<int>();
        for (int i = 0; i < _isOccupied.Count; i++)
        {
            if (!_isOccupied[i]) freeIndices.Add(i);
        }

        if (freeIndices.Count == 0) return;

        int randomIndex = freeIndices[Random.Range(0, freeIndices.Count)];
        Transform targetSlot = _holeSlots[randomIndex];

        // Spawn Mole vào thẳng làm con của Mole_Slot
        GameObject mole = PoolManager.Instance.SpawnFromPool(moleTag, targetSlot);

        if (mole != null)
        {
            _isOccupied[randomIndex] = true;
            RectTransform moleRect = mole.GetComponent<RectTransform>();
            moleRect.anchoredPosition = Vector2.zero; // Luôn ở giữa Slot
            moleRect.localScale = Vector3.one;

            // Truyền index để Mole biết khi nào chết thì giải phóng đúng slot đó
            mole.GetComponent<MoleUI>().SetMySlot(this, randomIndex);
        }
    }

    public void ReleaseSlot(int index)
    {
        if (index >= 0 && index < _isOccupied.Count)
            _isOccupied[index] = false;
    }
}