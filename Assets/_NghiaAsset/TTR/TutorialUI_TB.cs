using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class TutorialUI_TB : MonoBehaviour
{
    [Header("UI References")]
    public Image displayImage;
    public TextMeshProUGUI pageText;
    public TextMeshProUGUI descriptionText; 

    [Header("Buttons")]
    public Button nextButton;
    public Button prevButton;

    [Header("Data")]
    public List<Sprite> tutorialImages = new List<Sprite>();
    [TextArea(2, 5)]
    public List<string> descriptions = new List<string>(); 
    private int currentIndex = 0;

    void Start()
    {
        UpdateUI();

        nextButton.onClick.AddListener(NextImage);
        prevButton.onClick.AddListener(PrevImage);
    }

    void UpdateUI()
    {
        if (tutorialImages.Count == 0) return;

        displayImage.sprite = tutorialImages[currentIndex];
        pageText.text = (currentIndex + 1) + "/" + tutorialImages.Count;

        // 👇 Set mô tả
        if (descriptions != null && currentIndex < descriptions.Count)
        {
            descriptionText.text = descriptions[currentIndex];
        }
        else
        {
            descriptionText.text = ""; 
        }

        prevButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < tutorialImages.Count - 1;
    }

    public void NextImage()
    {
        if (currentIndex < tutorialImages.Count - 1)
        {
            currentIndex++;
            UpdateUI();
        }
    }

    public void PrevImage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateUI();
        }
    }
}