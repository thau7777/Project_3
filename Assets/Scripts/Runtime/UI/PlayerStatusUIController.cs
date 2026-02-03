using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject _hpGo;
    [SerializeField]
    private Slider _manaSlider;

    private Material HPMat;

    private void Awake()
    {
        HPMat = _hpGo.GetComponent<Image>().material;
    }
    public void InitializePlayerStatus(float currentHP, float maxHP, float currentMana, float maxMana)
    {
        UpdateHealth(currentHP, maxHP);
        UpdateMana(currentMana, maxMana);
    }

    public void UpdateHealth(float currentHP, float maxHP)
    {
        if(HPMat != null)
        {
            float hpRatio = currentHP / maxHP;
            HPMat.SetFloat("_FillLevel", hpRatio);
        }
    }
    public void UpdateMana(float currentMana, float maxMana)
    {
        if(_manaSlider != null)
        {
            _manaSlider.value = currentMana / maxMana;
        }
    }
}
