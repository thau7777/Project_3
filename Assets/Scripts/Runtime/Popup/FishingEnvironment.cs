using MyRule;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FishingEnvironment : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private List<Sprite> enviSprites;
    [SerializeField] private bool isTest;


    private void Awake()
    {
        if (background == null)
        {
            background = GetComponent<Image>();
        }
        background.sprite = enviSprites[0];

        if(isTest) return;
        SwitchEnvi();
    }
    private void SwitchEnvi()
    {
        EMap mapType = MatchManager.Instance.MatchData.MapType;
        switch (mapType)
        {
            case EMap.GreenLand:
                background.sprite = enviSprites[0];
                break;
            case EMap.Desert:
                background.sprite = enviSprites[1];
                break;
            case EMap.IceLand:
                background.sprite = enviSprites[2];
                break;

        }
    }
}
    
        