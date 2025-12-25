using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Turnbase/Item")]
    public class Tb_Item : ScriptableObject
    {
        public string itemName;
        public string description;
        public int value;
        public Sprite icon;
    }

}