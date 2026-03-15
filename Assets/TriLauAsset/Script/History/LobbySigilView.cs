using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class LobbySigilView : MonoBehaviour
    {
        [SerializeField] private Image sigilImg;

        public void SetSigil(SigilData sigilData)
        {
            SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);

            if (sigilSO != null)
            {
                sigilImg.sprite = sigilSO.sigilIcon;
            }
        }
    }
}