using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class UI_MoleSpawner : MonoBehaviour
{
    public string moleTag = "Mole";

    [Header("Hole Prefab")]
    public RectTransform holePrefab; // KÉO 1 HOLE MẪU VÀO ĐÂY

    [Header("Spawn Config")]
    public int minHole = 6;
    public int maxHole = 10;
    public float radius = 250f;
    public float spawnDelay = 1.5f;

    [Header("Timer Config")]
    public float gameDuration = 60f;
    public float timer;

    [SerializeField] private int score = 0;

    private List<RectTransform> holes = new();
    private List<Transform> _slots = new();
    private List<bool> _isOccupied = new();

    void OnEnable()
    {
        RandomizeHolePositions();
        StartGame();
    }
    void Awake()
    {
        GenerateHoles();     // chỉ chạy 1 lần
        CacheSlots();
    }

    void OnDisable()
    {
        CancelInvoke();
        ReleaseAllSlots();
    }
    void StartGame()
    {
        timer = gameDuration;

        ReleaseAllSlots();
        CancelInvoke();

        InvokeRepeating(nameof(SpawnMole), 0.5f, spawnDelay);
        Invoke(nameof(EndGame), gameDuration);
    }
    void EndGame()
    {
        CancelInvoke();

        // Ẩn toàn bộ mole đang có
        foreach (Transform slot in _slots)
        {
            foreach (Transform child in slot)
                child.gameObject.SetActive(false);
        }

        Debug.Log("Mini game kết thúc! Điểm: " + score);
    }

    public void AddScore(int value)
    {
        score += value;
    }
    void GenerateHoles()
    {
        holes.Clear();

        int count = Random.Range(6, 9);

        for (int i = 0; i < count; i++)
        {
            RectTransform hole = Instantiate(holePrefab, transform);
            hole.gameObject.SetActive(true);
            holes.Add(hole);
        }
    }

    void CacheSlots()
    {
        _slots.Clear();
        _isOccupied.Clear();

        foreach (var hole in holes)
        {
            Transform slot = hole.Find("Mole_Slot");
            _slots.Add(slot != null ? slot : hole);
            _isOccupied.Add(false);
        }
    }

    void RandomizeHolePositions()
    {
        float startAngle = Random.Range(0f, Mathf.PI * 2f);

        for (int i = 0; i < holes.Count; i++)
        {
            float angle = startAngle + (i * Mathf.PI * 2f / holes.Count);
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            holes[i].anchoredPosition = new Vector2(x, y);
        }
    }

    void SpawnMole()
    {
        List<int> free = new();
        for (int i = 0; i < _isOccupied.Count; i++)
            if (!_isOccupied[i]) free.Add(i);

        if (free.Count == 0) return;

        int index = free[Random.Range(0, free.Count)];
        Transform slot = _slots[index];

        GameObject mole = PoolManager.Instance.SpawnFromPool(moleTag, slot);
        if (mole == null) return;

        _isOccupied[index] = true;

        RectTransform rect = mole.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;

        mole.GetComponent<MoleUI>().Init(this, index);
    }

    public void ReleaseSlot(int index)
    {
        if (index >= 0 && index < _isOccupied.Count)
            _isOccupied[index] = false;
    }

    void ReleaseAllSlots()
    {
        for (int i = 0; i < _isOccupied.Count; i++)
            _isOccupied[i] = false;
    }
}