using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Turnbase
{
    public class ItemEntryUI : MonoBehaviour
    {
        public Button ItemButton;
        public Image ItemIcon;
        public TextMeshProUGUI ItemNameText;
        public TextMeshProUGUI ItemQuantityText;

        public Tb_Item ItemData { get; private set; }

        public void SetUp(Tb_Item item, Action<Tb_Item> clickAction)
        {
            ItemData = item;

            if (ItemIcon != null && item.icon != null)
            {
                ItemIcon.sprite = item.icon;
                ItemIcon.color = Color.white;
            }

            if (ItemNameText != null)
            {
                ItemNameText.text = item.itemName;
            }

            if (ItemQuantityText != null)
            {
                ItemQuantityText.text = item.quantity.ToString();
            }

            ItemButton.onClick.RemoveAllListeners();
            ItemButton.onClick.AddListener(() => clickAction.Invoke(ItemData));
        }

        public void SelectThisItem()
        {
            ItemButton.onClick.Invoke();
        }

    }



}