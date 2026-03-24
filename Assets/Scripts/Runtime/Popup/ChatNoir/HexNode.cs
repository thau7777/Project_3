using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexNode : MonoBehaviour
{
    public int row, col;
    public bool isBlocked;

    [Header("UI References")]
    [SerializeField] private Image displayImage; 
    [SerializeField] private Image backGroundImage;
    [SerializeField] private Button btn;

    [Header("Sprites Settings")]
    [SerializeField] private List<Sprite> grassSprites; 
    [SerializeField] private List<Sprite> desertSprites; 
    [SerializeField] private List<Sprite> iceSprites;
    [SerializeField] private List<Sprite> nodeSprites;


    public void Init(int r, int c, System.Action<HexNode> onClickAction)
    {
        row = r;
        col = c;
        isBlocked = false;

        // Reset về Sprite trống (index 0)
        if (iceSprites.Count > 0)
            displayImage.sprite = iceSprites[0];
        backGroundImage.sprite = nodeSprites[2];

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => onClickAction(this));

        gameObject.name = $"Node_{r}_{c}";
    }

    public void SetAsWall()
    {
        isBlocked = true;
        // Đổi sang Sprite tường (index 1)
        if (iceSprites.Count > 1)
            displayImage.sprite = iceSprites[1];
        else
            Debug.LogError("Chưa kéo đủ 2 Sprite vào list nodeSprites!");
    }
    
}