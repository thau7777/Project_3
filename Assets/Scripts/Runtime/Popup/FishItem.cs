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
    [Tooltip("Bật nếu sprite mặc định quay sang PHẢI")]
    public bool facingRightByDefault = false;
    
    
    [SerializeField] private float startX;
    [SerializeField] private int direction = 1;
    [SerializeField] private RectTransform rt;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float timeOffset;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        rb = GetComponent<Rigidbody2D>();

        GetRandomNumber();
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

        //if (itemType == FishingItemType.Fish)
        //    rb.simulated = false;
        //else
        //    rb.simulated = true;
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
            Mathf.Sin(t) * 0.3f,
            Mathf.Cos(t * 0.5f) * 0.2f
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
                FishingUI.instance.fishTime *= 0.05f;
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
                break;
            case FishingItemType.Trash:
                FishingGameManager.Instance.AddScore(-scoreValue);
                break;
        }
        StartCoroutine(HideAndDestroy());
    }

    IEnumerator HideAndDestroy()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    public void OnEscape()
    {
        state = FishState.Swimming;
        rb.simulated = true;
    }
}