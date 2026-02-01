using UnityEngine;

namespace Turnbase
{
    public enum DoTType { Burn, Poison, All } 

    [CreateAssetMenu(fileName = "NewDoTBoost", menuName = "Battle/Passive/DoT Boost")]
    public class Passive_DoTBoost : SkillPassive
    {
        public DoTType targetType; 
        [Range(0, 2)] public float multiplier = 0.2f;

        public int GetBoostedDamage(int originalDamage, DoTType currentType)
        {
            if (targetType == DoTType.All || targetType == currentType)
            {
                return Mathf.RoundToInt(originalDamage * (1f + multiplier));
            }
            return originalDamage;
        }
    }
}