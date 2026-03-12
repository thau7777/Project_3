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
        [SerializeField] private GameObject soldObj;

        private void Start()
        {
            btn.onClick.AddListener(OnClick);
        }

        public void SetUp(ItemSO item)
        {
            itemSo = item;
            icon.sprite = item.icon;
            priceTxt.text = item.price.ToString();
            soldObj.SetActive(false);
        }

        private void OnClick()
        {
            var rune = RuneManger.Instance.CurrentRuneAmount;

            var hasSlot = ItemStorageManager.Instance.HasEmptyItemStorageSlot();

            if (hasSlot && rune > itemSo.price)
            {
                ItemStorageManager.Instance.AddItemToStorage(itemSo);
                soldObj.SetActive(true);
                
                EventBus<ReceiveRuneEvent>.Raise(new ReceiveRuneEvent(-itemSo.price));
            }
            else
            {
                Debug.Log("Cant add item");
            }
        }
    }
}