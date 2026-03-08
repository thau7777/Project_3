using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyRule
{
    public class ItemSlotView : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private ItemSO item;
        [SerializeField] private Image icon;
        [SerializeField] private GameObject hightLightObj;

        public void SetItem(ItemSO item)
        {
            this.item = item;
            icon.sprite = item.icon;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            hightLightObj.SetActive(false);
        }

        public void OnSelect(BaseEventData eventData)
        {
            hightLightObj.SetActive(true);
        }
    }
}