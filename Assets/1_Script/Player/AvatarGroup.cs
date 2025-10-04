using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarGroup : MonoBehaviour
{
    public Image avatar;
    public Image hpBar;
    public Image mpBar;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;

    private Character ownerCharacter;


    private void Awake()
    {
        ownerCharacter = GetComponentInParent<Character>();
    }
    public void SetOwner(Character character)
    {
        ownerCharacter = character;
    }

    private void Start()
    {
        if (ownerCharacter != null)
        {
            UpdateUI(ownerCharacter.stats);
        }
        else
        {
            ownerCharacter = GetComponentInParent<Character>();
            if (ownerCharacter != null)
            {
                UpdateUI(ownerCharacter.stats);
            }
        }
    }

    public void UpdateUI(CharacterStats stats)
    {
        avatar.sprite = stats.Avatar;
        hpBar.fillAmount = (float)stats.currentHP / stats.maxHP;
        mpBar.fillAmount = (float)stats.currentMP / stats.maxMP;
        hpText.text = $"{stats.currentHP} / {stats.maxHP}";
        mpText.text = $"{stats.currentMP} / {stats.maxMP}";
    }

}
