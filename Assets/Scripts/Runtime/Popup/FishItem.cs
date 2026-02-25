using UnityEngine;

public enum FishingItemType
{
    Fish,
    Trash
}

public enum FishState
{
    Swimming,
    Hooked
}

public class FishItem : MonoBehaviour
{
    public FishingItemType itemType;
    public FishState state;

    public int scoreValue = 100;

    public string poolTag; // ⚠️ QUAN TRỌNG

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init()
    {
        state = FishState.Swimming;
        rb.simulated = true;
    }

    public void OnHooked()
    {
        if (state != FishState.Swimming) return;

        state = FishState.Hooked;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
    }

    public void OnCaught()
    {
        if (itemType == FishingItemType.Fish)
        {
            FishingGameManager.Instance.AddScore(scoreValue);
        }

        PoolManager.Instance.Despawn(poolTag, gameObject);
    }

    public void OnEscape()
    {
        state = FishState.Swimming;
        rb.simulated = true;
    }
}