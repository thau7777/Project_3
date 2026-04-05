using System.Collections;
using UnityEngine;

public enum FishingItemType
{
    Fish,
    Trash,
    Treasure
}

public enum FishState
{
    Swimming,
    Hooked
}

public class FishItem : MonoBehaviour
{
    [Header("Config")]
    public FishingItemType itemType;
    public int scoreValue = 100;

    [Header("State")]
    public FishState state;

    [Header("Fish Move")]
    public float moveSpeed;
    public float moveRange;

    [Header("Sprite Direction")]
    [Tooltip("Sprite is facing to the RIGHT")]
    public bool facingRightByDefault = false;
    
    
    [SerializeField] private float startX;
    [SerializeField] private int direction = 1;
    [SerializeField] private RectTransform rt;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float timeOffset;
    [SerializeField] private string poolTag;
    [SerializeField] private string bubblePoolTag = "Bubble";
    [SerializeField] private float bubbleSpawnInterval = 1.5f;
    

    Coroutine bubbleRoutine;


    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        rb = GetComponent<Rigidbody2D>();

        GetRandomNumber();
        poolTag = itemType.ToString();
        bubbleRoutine = StartCoroutine(SpawnBubbleRoutine());
    }

    private void Update()
    {
        if (state != FishState.Swimming) return;

        if (itemType == FishingItemType.Fish)
        {
            MoveFish();
        }
        else
        {
            MoveFloating();
        }

    }
    public void Init()
    {
        state = FishState.Swimming;
        gameObject.SetActive(true);

        startX = rt.anchoredPosition.x;

        direction = Random.value > 0.5f ? 1 : -1;
        ApplyFlip();
        
        
        

    }
    public void GetRandomNumber()
    {
        moveSpeed = Random.Range(30f, 60f);
        timeOffset = Random.Range(0f, 10f);
        moveRange = Random.Range(200f, 400f);
    }
    #region Movement
    void MoveFish()
    {
        float x = rt.anchoredPosition.x + direction * moveSpeed * Time.deltaTime;

        float leftLimit = startX - moveRange;
        float rightLimit = startX + moveRange;

        if (x < leftLimit)
        {
            x = leftLimit;
            direction = 1;
            ApplyFlip();
        }
        else if (x > rightLimit)
        {
            x = rightLimit;
            direction = -1;
            ApplyFlip();
        }

        rt.anchoredPosition = new Vector2(x, rt.anchoredPosition.y);
    }
    void ApplyFlip()
    {
        Vector3 scale = transform.localScale;

        // Nếu sprite mặc định quay trái thì đảo logic
        int visualDir = facingRightByDefault ? direction : -direction;

        scale.x = Mathf.Abs(scale.x) * visualDir;
        transform.localScale = scale;
    }
    void MoveFloating()
    {
        float t = Time.time + timeOffset;

        rb.linearVelocity = new Vector2(
            Mathf.Sin(t) * 3f,
            Mathf.Cos(t * 5f) * 2f
        );
    }
    #endregion
    public void OnHooked()
    {
        if (state != FishState.Swimming) return;

        state = FishState.Hooked;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        
        switch (itemType)
        {
            case FishingItemType.Fish:
                FishingUI.instance.fishTime *= 0.2f;
                break;

            case FishingItemType.Treasure:
                FishingUI.instance.fishTime *= 0.5f;
                break;
            case FishingItemType.Trash:
                FishingUI.instance.fishTime *= 0f;
                break;
        }
    }

    public void OnCaught()
    {
        switch(itemType)
        {
            case FishingItemType.Fish:
                FishingGameManager.Instance.AddScore(scoreValue);
                break;
            case FishingItemType.Treasure:
                FishingGameManager.Instance.AddScore(scoreValue * 5);
                Debug.Log("Treasure");
                break;
            case FishingItemType.Trash:
                FishingGameManager.Instance.AddScore(-scoreValue);
                break;
        }
        FishingSpawner.instance.fishCount--;
        if (bubbleRoutine != null)
            StopCoroutine(bubbleRoutine);
        StartCoroutine(ReturnToPool());
        
    }

    IEnumerator ReturnToPool()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);

        PoolManager.Instance.Despawn(poolTag, gameObject);
    }

    public void OnEscape()
    {
        state = FishState.Swimming;
        rb.simulated = true;
    }
    void SpawnBubble()
    {
        GameObject bubble = PoolManager.Instance.SpawnFromPool(bubblePoolTag, gameObject.transform);


        float scale = Random.Range(0.05f, 0.3f);
        bubble.transform.localScale = Vector3.one * scale;
    }
    IEnumerator SpawnBubbleRoutine()
    {
        while (state == FishState.Swimming)
        {
            float waitTime = Random.Range(10f, 15f);
            yield return new WaitForSeconds(waitTime);
            int bubbleCount = Random.Range(1, 4);
            for (int i = 0; i < bubbleCount; i++)
            {
                if (state != FishState.Swimming)
                    yield break;
                SpawnBubble();
                yield return new WaitForSeconds(Random.Range(0.1f, 2f));
            }
        }
    }
}