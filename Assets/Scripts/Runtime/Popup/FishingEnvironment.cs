using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FishingEnvironment : MonoBehaviour
{
    [SerializeField] private Image background;
    [SerializeField] private List<Sprite> enviSprites;


    private void Awake()
    {
        if (background == null)
        {
            background = GetComponent<Image>();
        }
        background.sprite = enviSprites[0];
    }
    private void SwitchEnvi()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "GreenlandScene":
                background.sprite = enviSprites[0];
                break;
            case "DesertScene":
                background.sprite = enviSprites[1];
                break;
            case "IcelandScene":
                background.sprite = enviSprites[2];
                break;

        }
    }
}
    
        