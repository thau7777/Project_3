using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule
{
    public class StoreItemView : MonoBehaviour
    {
        [SerializeField] private ItemSO itemSo;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI priceTxt;
        [SerializeField] private Button btn;

        private void Start()
        {
            btn.onClick.AddListener(OnClick);
        }

        public void SetUp(ItemSO item)
        {
            itemSo = item;
            icon.sprite = item.icon;
            priceTxt.text = item.price.ToString();
        }

        private void OnClick()
        {
            ItemStorageManager.Instance.AddItemToStorage(itemSo);
        }
    }
}