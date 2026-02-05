using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "NewBasicAttackBoost", menuName = "Battle/Passive/Basic Attack Boost")]
    public class Passive_BasicAttackBoost : SkillPassive
    {
        [Header("Chỉ số cộng thêm")]
        public float percentBonus = 0.2f; 
        public int flatBonus = 10;     

        public int ApplyBoost(int currentDamage)
        {
            return Mathf.RoundToInt(currentDamage * (1 + percentBonus)) + flatBonus;
        }
    }
}