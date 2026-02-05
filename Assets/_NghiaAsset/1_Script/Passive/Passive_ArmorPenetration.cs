using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "NewArmorPen", menuName = "Battle/Passive/Armor Penetration")]
    public class Passive_ArmorPenetration : SkillPassive
    {
        [Range(0, 1)] public float penetrationPercent = 0.2f;

        public int GetReducedDefense(int originalDefense)
        {
            int reducedDefense = Mathf.RoundToInt(originalDefense * (1f - penetrationPercent));
            return Mathf.Max(0, reducedDefense);
        }
    }
}