using UnityEngine;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private Image icon;

        public bool IsEmpty => icon != null;

        public void SetIcon(Item item)
        {
            icon.sprite = item.Icon;
        }
    }
}