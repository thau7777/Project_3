using UnityEngine;

namespace MyRule
{
    public static class GetMultiplier
    {
        public static float GetStatMultiplier(int statValue)
        {
            return (float)100 / (100 - statValue);
        }
    }
}