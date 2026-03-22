using System.Collections.Generic;
using UnityEngine;

namespace MyRule.Utils
{
    public static class RandomISigilPicker
    {
        /// <summary>
        /// Lấy random KHÔNG TRÙNG theo WEIGHT
        /// </summary>
        public static List<SigilData> GetRandomWeightedUnique(List<SigilData> source, int count)
        {
            List<SigilData> result = new List<SigilData>();

            if (source == null || source.Count == 0 || count <= 0)
                return result;

            List<SigilData> temp = new List<SigilData>(source);

            while (result.Count < count && temp.Count > 0)
            {
                float totalWeight = 0f;

                foreach (var item in temp)
                {
                    totalWeight += item.Weight;
                }

                float randomPoint = Random.Range(0f, totalWeight);

                float current = 0f;

                for (int i = 0; i < temp.Count; i++)
                {
                    current += temp[i].Weight;

                    if (randomPoint <= current)
                    {
                        result.Add(temp[i]);
                        temp.RemoveAt(i);
                        break;
                    }
                }
            }

            return result;
        }
    }
}