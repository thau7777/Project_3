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

    Rigidbody2D rb;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init()
    {
        state = FishState.Swimming;
        rb.simulated = true;
        gameObject.SetActive(true);
    }

    public void OnHooked()
    {
        if (state != FishState.Swimming) return;

        state = FishState.Hooked;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        if (itemType == FishingItemType.Fish)
        {
            FishingUI.instance.fishTime *= 0.2f;
        }
        if (itemType == FishingItemType.Treasure)
        {
            FishingUI.instance.fishTime *= 0.05f;
        }
    }

    public void OnCaught()
    {
        if(itemType == FishingItemType.Fish)
        {
            FishingGameManager.Instance.AddScore(scoreValue);
        }
        if (itemType == FishingItemType.Treasure)
        {
            FishingGameManager.Instance.AddScore(scoreValue*5);
        }
        else
        {
            FishingGameManager.Instance.AddScore(scoreValue);
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