using UnityEngine;
using UnityEngine.UI;

public class ArrowManager : MonoBehaviour
{
    [Header("Arrow Data")]
    public ArrowType currentArrow;

    [Header("UI")]
    public Image arrowImage;
    public Sprite leftSprite;
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite rightSprite;

    [Header("Timing")]
    public float perfectTime = 0.2f;
    public float goodTime = 0.4f;

    private float appearTime;

    void Start()
    {
        NextArrow();
    }

    //public void CheckInput(ArrowType input)
    //{
    //    float deltaTime = Time.time - appearTime;

    //    if (input == currentArrow)
    //    {
    //        if (deltaTime <= perfectTime)
    //        {
    //            Debug.Log("PERFECT!");
    //        }
    //        else if (deltaTime <= goodTime)
    //        {
    //            Debug.Log("GOOD!");
    //        }
    //        else
    //        {
    //            Debug.Log("BAD!");
    //        }

    //        NextArrow();
    //    }
    //    else
    //    {
    //        Debug.Log("MISS!");
    //    }
    //}

    public void NextArrow()
    {
        currentArrow = (ArrowType)Random.Range(0, 4);
        appearTime = Time.time;
        UpdateArrowUI();
    }

    public void UpdateArrowUI()
    {
        switch (currentArrow)
        {
            case ArrowType.Left:
                arrowImage.sprite = leftSprite;
                break;
            case ArrowType.Up:
                arrowImage.sprite = upSprite;
                break;
            case ArrowType.Down:
                arrowImage.sprite = downSprite;
                break;
            case ArrowType.Right:
                arrowImage.sprite = rightSprite;
                break;
        }
    }
}
