using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MyRule
{
    public enum ERarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythic,
    }


    [Serializable]
    public class SigilData
    {
        [JsonProperty] private string id;
        [JsonProperty] private string name;
        [JsonProperty] private int baseDamage;
        [JsonProperty] private int manaCost;
        [JsonProperty] private SigilType sigilType;
        [JsonProperty] private ERarity rarity;
        [JsonProperty] private float weight;
        [JsonProperty] private EKeyBinding eKeyBinding;

        [JsonIgnore] public string Id => id;
        [JsonIgnore] public string Name => name;
        [JsonIgnore] public int BaseDamage => baseDamage;
        [JsonIgnore] public int ManaCost => manaCost;
        [JsonIgnore] public SigilType SigilType => sigilType;
        [JsonIgnore] public ERarity Rarity => rarity;
        [JsonIgnore] public EKeyBinding EKeyBinding => eKeyBinding;

        public SigilData(string id, SigilType sigilType, string name, int baseDamage, int manaCost, ERarity rarity, EKeyBinding eKeyBinding)
        {
            this.id = id;
            this.name = name;
            this.baseDamage = baseDamage;
            this.manaCost = manaCost;
            this.sigilType = sigilType;
            this.rarity = rarity;
            this.eKeyBinding = eKeyBinding;

            switch (rarity)
            {
                case ERarity.Common:
                    this.weight = 50;
                    break;
                case ERarity.Uncommon:
                    this.weight = 30;
                    break;
                case ERarity.Rare:
                    this.weight = 12;
                    break;
                case ERarity.Epic:
                    this.weight = 6;
                    break;
                case ERarity.Legendary:
                    this.weight = 1.8f;
                    break;
                case ERarity.Mythic:
                    this.weight = 0.2f;
                    break;
            }
        }

        public float GetWeight(int steps)
        {
            if (steps >= 35)
                return 0;

            float t = Mathf.Clamp01(steps / 36f);

            // Late spike zone (32–34)
            bool isLateSpike = steps >= 32;

            switch (rarity)
            {
                case ERarity.Common:
                    return isLateSpike
                        ? Mathf.Lerp(20, 5, t)
                        : Mathf.Lerp(50, 20, t);

                case ERarity.Uncommon:
                    return isLateSpike
                        ? Mathf.Lerp(15, 8, t)
                        : Mathf.Lerp(30, 18, t);

                case ERarity.Rare:
                    return isLateSpike
                        ? Mathf.Lerp(18, 25, t)
                        : Mathf.Lerp(12, 18, t);

                case ERarity.Epic:
                    return isLateSpike
                        ? Mathf.Lerp(14, 22, t)
                        : Mathf.Lerp(6, 14, t);

                case ERarity.Legendary:
                    return isLateSpike
                        ? Mathf.Lerp(8, 18, SpikeCurve(t)) 
                        : Mathf.Lerp(1.8f, 8f, EaseOut(t));

                case ERarity.Mythic:
                    return isLateSpike
                        ? Mathf.Lerp(2f, 12f, SpikeCurve(t)) 
                        : Mathf.Lerp(0.2f, 3f, LateCurve(t));
            }

            return 0;
        }

        private float EaseOut(float t)
        {
            return 1 - Mathf.Pow(1 - t, 2);
        }

        private float LateCurve(float t)
        {
            return Mathf.Pow(t, 3);
        }

        private float SpikeCurve(float t)
        {
            return Mathf.Pow(t, 5);
        }
    }
}