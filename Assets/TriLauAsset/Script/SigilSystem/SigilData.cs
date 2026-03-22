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
        [JsonIgnore] public float Weight => weight;
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
    }
}