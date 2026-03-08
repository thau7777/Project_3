using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private Image icon;

        public bool IsEmpty = true;

        public void SetIcon(ItemSO item)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = item.icon;
            IsEmpty = false;
        }
    }
}