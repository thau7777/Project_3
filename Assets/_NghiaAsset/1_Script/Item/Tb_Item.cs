using UnityEngine;
using MyRule;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Turnbase/Item")]
    public class Tb_Item : ScriptableObject
    {
        public string itemName;
        public string description;
        public int value;
        public int quantity;
        public Sprite icon;

        public FlyweightSettings_TB effect;

        public ItemType type;



    }

}