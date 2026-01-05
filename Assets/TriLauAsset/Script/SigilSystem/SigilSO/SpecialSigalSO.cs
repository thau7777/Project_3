using UnityEngine;
using UnityEngine.Splines.ExtrusionShapes;

namespace MyRule
{
    [CreateAssetMenu(fileName = "SpecialSigalSO", menuName = "Sigil/SpecialSigalSO")]
    public class SpecialSigalSO : ScriptableObject
    {
        [Header("Sigil Info")]
        public Texture2D sigilIcon;
        public string sigilName;
        [TextArea(3, 10)]
        public string sigilDesTD;
        public string sigilDesTB;
        public int rarity;

        [Header("Sigil Stats")]
        public int diceFaceIncrease;
        public int rewardShapeDropRate;
        public int goldIncrease;

        [Header("Sigil Requirements")]
        public int numbOfRolls;
        public int price;
    }
}