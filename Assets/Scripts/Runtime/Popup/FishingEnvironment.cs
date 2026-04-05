using MyRule;
using System;
using System.Collections.Generic;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FishingEnvironment : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private List<Sprite> enviSprites;
    [SerializeField] private List<Sprite> waterSprite;
    [SerializeField] private bool isTest;
    [SerializeField] private Image water;


    private void Awake()
    {
        if (background == null && water == null)
        {
            background = GetComponent<Image>();
            water = GetComponent<Image>();
        }
        
        if (isTest)
        {
            background.sprite = enviSprites[2];
            water.sprite = waterSprite[2];
            return;
        }
        else
        {
            SwitchEnvi();
        }
        
    }
    private void SwitchEnvi()
    {
        EMap mapType = MatchManager.Instance.MatchData.MapType;
        switch (mapType)
        {
            case EMap.GreenLand:
                background.sprite = enviSprites[0];
                water.sprite = waterSprite[0];
                break;
            case EMap.Desert:
                background.sprite = enviSprites[1];
                water.sprite = waterSprite[1];
                break;
            case EMap.IceLand:
                background.sprite = enviSprites[2];
                water.sprite = waterSprite[2];
                break;

        }
    }
}
    
        