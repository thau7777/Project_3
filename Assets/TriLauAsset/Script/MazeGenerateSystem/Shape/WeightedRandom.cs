using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class WeightedRandom : MonoBehaviour
    {
        public static WeightedRandom Instance;
        public List<ShapeSO> shapeList;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public ShapeSO GetWeightedRandom()
        {
            int totalWeight = 0;
            foreach (var w in shapeList)
                totalWeight += w.weight;

            int random = Random.Range(0, totalWeight);
            int current = 0;

            foreach (var w in shapeList)
            {
                current += w.weight;
                if (random < current)
                    return w;
            }

            return null;
        }
    }
}