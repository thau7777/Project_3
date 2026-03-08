using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule
{
    public class SigilView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI keyTxt;
        [SerializeField] private ActiveSigilType key;
        [SerializeField] private SigilSO sigilSO;

        public bool IsEmpty => icon != null;
        public ActiveSigilType Key => key;

        public void SetSigil(SigilSO sigilSO)
        {
            this.sigilSO = sigilSO;
            icon.gameObject.SetActive(true);
            icon.sprite = sigilSO.sigilIcon;
            keyTxt.text = sigilSO.activeSigilType.ToString();
        }
    }
}