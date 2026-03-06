using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule
{
    public class StoreItemView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI priceTxt;

        public void SetUp(Item item)
        {
            icon.sprite = item.Icon;
            priceTxt.text = item.Price.ToString();
        }
    }
}