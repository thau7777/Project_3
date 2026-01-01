using UnityEngine;

namespace MyRule
{
    public enum ShapeType
    {
        None,
        Creeeps,
        Boss,
        Trap,
        Treasure,
        Recovery,
        Sigil,
        Shop,
        MiniGame
    }

    [CreateAssetMenu(fileName = "ShapeSO", menuName = "Scriptable Objects/ShapeSO")]
    public class ShapeSO : ScriptableObject
    {
        public ShapeType shapeType;
        public Sprite shapeIcon;
        public int weight;
    }
}