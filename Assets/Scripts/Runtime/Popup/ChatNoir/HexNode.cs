using MyRule;
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
    [SerializeField] private List<Sprite> wallSprites;

    [SerializeField] private bool isTest;


    public void Init(int r, int c, System.Action<HexNode> onClickAction)
    {
        row = r;
        col = c;
        isBlocked = false;

        if(isTest)
        {
            if (grassSprites.Count > 0)
            {
                displayImage.sprite = grassSprites[0];
                backGroundImage.sprite = wallSprites[0];
            }
        }
        else
        {
            if (grassSprites.Count > 0)
            {
                EMap mapType = MatchManager.Instance.MatchData.MapType;

                switch (mapType)
                {
                    case EMap.GreenLand:
                        displayImage.sprite = grassSprites[0];
                        backGroundImage.sprite = wallSprites[0];
                        break;
                    case EMap.Desert:
                        displayImage.sprite = desertSprites[0];
                        backGroundImage.sprite = wallSprites[1];
                        break;
                    case EMap.IceLand:
                        displayImage.sprite = iceSprites[0];
                        backGroundImage.sprite = wallSprites[2];
                        break;
                }

            }
        }
        //displayImage.sprite = grassSprites[0];
        //backGroundImage.sprite = nodeSprites[0];

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => onClickAction(this));

        gameObject.name = $"Node_{r}_{c}";
    }

    public void SetAsWall()
    {
        isBlocked = true;

        if (grassSprites.Count > 1)
        {
            EMap mapType = MatchManager.Instance.MatchData.MapType;
            switch(mapType)
            {
                case EMap.GreenLand:
                    displayImage.sprite = grassSprites[1];
                    break;
                case EMap.Desert:
                    displayImage.sprite = desertSprites[1];
                    break;
                case EMap.IceLand:
                    displayImage.sprite = iceSprites[1];
                    break;
            }
        }
        
    }
    
}