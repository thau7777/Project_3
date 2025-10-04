using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnOrderIcon : MonoBehaviour
{
    [Header("UI References")]
    public Image avatarImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI actionGaugeText;

    [HideInInspector] public Character characterOwner;

    public void UpdateIcon(Character character)
    {
        characterOwner = character;

        if (avatarImage != null && character.stats.Avatar != null)
        {
            avatarImage.sprite = character.stats.Avatar;
        }

        if (nameText != null)
        {
            nameText.text = character.gameObject.name;
        }

        if (actionGaugeText != null)
        {
            actionGaugeText.text = Mathf.RoundToInt(character.actionGauge).ToString();
        }
    }
}