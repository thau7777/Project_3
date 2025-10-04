using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatsUI : MonoBehaviour
{
    public Image hpBarFill;
    public Image mpBarFill;


    private Character ownerCharacter;

    void Awake()
    {
        ownerCharacter = GetComponentInParent<Character>();
    }

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (ownerCharacter != null)
        {
            CharacterStats stats = ownerCharacter.stats;

            if (hpBarFill != null)
            {
                hpBarFill.fillAmount = (float)stats.currentHP / stats.maxHP;
            }

            if (mpBarFill != null)
            {
                mpBarFill.fillAmount = (float)stats.currentMP / stats.maxMP;
            }

        }
    }
}