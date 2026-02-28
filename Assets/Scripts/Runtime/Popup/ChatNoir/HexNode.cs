using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexNode : MonoBehaviour
{
    public int row, col;
    public bool isBlocked;

    [Header("UI References")]
    [SerializeField] private Image displayImage; // Kéo Image component vào đây
    [SerializeField] private Button btn;

    [Header("Sprites Settings")]
    [SerializeField] private List<Sprite> nodeSprites; // Index 0: Trống, Index 1: Tường

    public void Init(int r, int c, System.Action<HexNode> onClickAction)
    {
        row = r;
        col = c;
        isBlocked = false;

        // Reset về Sprite trống (index 0)
        if (nodeSprites.Count > 0)
            displayImage.sprite = nodeSprites[0];

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => onClickAction(this));

        gameObject.name = $"Node_{r}_{c}";
    }

    public void SetAsWall()
    {
        isBlocked = true;
        // Đổi sang Sprite tường (index 1)
        if (nodeSprites.Count > 1)
            displayImage.sprite = nodeSprites[1];
        else
            Debug.LogError("Chưa kéo đủ 2 Sprite vào list nodeSprites!");
    }
    
}