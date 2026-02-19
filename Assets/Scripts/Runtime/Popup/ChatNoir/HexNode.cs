using UnityEngine;
using UnityEngine.UI;

public class HexNode : MonoBehaviour
{
    public int row, col;
    public bool isBlocked;
    [SerializeField] private Image nodeImage;
    [SerializeField] private Button btn;

    public Color normalColor = Color.white;
    public Color wallColor = Color.gray;

    public void Init(int r, int c, System.Action<HexNode> onClickAction)
    {
        row = r;
        col = c;
        isBlocked = false;
        nodeImage.color = normalColor;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => onClickAction(this));

        gameObject.name = $"Node_{r}_{c}";
    }

    public void SetAsWall()
    {
        isBlocked = true;
        nodeImage.color = wallColor;
    }
}