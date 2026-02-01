using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "NewMagicPen", menuName = "Battle/Passive/Magic Penetration")]
    public class Passive_MagicPenetration : SkillPassive
    {
        [Range(0, 1)] public float penetrationPercent = 0.2f; 

        public int GetReducedDefense(int originalDefense)
        {
            int reducedDefense = Mathf.RoundToInt(originalDefense * (1f - penetrationPercent));
            return Mathf.Max(0, reducedDefense); 
        }
    }
}