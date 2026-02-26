using UnityEngine;
using UnityEngine.UI;

public class ArrowManager : MonoBehaviour
{
    public ArrowType currentArrow;

    public Image arrowImage;
    public Sprite leftSprite;
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite rightSprite;

    public void NextArrow()
    {
        currentArrow = (ArrowType)Random.Range(0, 4);
        UpdateArrowUI();
    }

    void UpdateArrowUI()
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