using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
    public class SigilCollectionData
    {
        [JsonProperty] protected Dictionary<string, SigilData> acctiveSigils;
        [JsonProperty] protected Dictionary<string, SigilData> passiveSigils;

        [JsonIgnore] public int Count => acctiveSigils.Count + passiveSigils.Count;

        [JsonIgnore] public Dictionary<string, SigilData> ActiveSigils => acctiveSigils;
        [JsonIgnore] public Dictionary<string, SigilData> PassiveSigils => passiveSigils;

        public SigilCollectionData()
        {
            acctiveSigils = new Dictionary<string, SigilData>();
            passiveSigils = new Dictionary<string, SigilData>();
        }

        public void AddSigil(SigilData sigil)
        {
            if (sigil.SigilType == SigilType.Active)
            {
                if (!acctiveSigils.ContainsKey(sigil.Id))
                    acctiveSigils.Add(sigil.Id, sigil);
            }
            else
            {
                if (!passiveSigils.ContainsKey(sigil.Id))
                    passiveSigils.Add(sigil.Id, sigil);
            }
        }

        public void RemoveSigil(SigilData sigil)
        {
            if (sigil.SigilType == SigilType.Active)
            {
                acctiveSigils.Remove(sigil.Id);
            }
            else
            {
                passiveSigils.Remove(sigil.Id);
            }
        }

        public void SetActiveSigils(Dictionary<string, SigilData> sigils) => acctiveSigils = sigils;

        public void SetPassiveSigils(Dictionary<string, SigilData> sigils) => passiveSigils = sigils;
    }

    [Serializable]
    public class SigilsInMatchData : SigilCollectionData
    {
        public SigilsInMatchData()
        {
            acctiveSigils = new Dictionary<string, SigilData>();
            passiveSigils = new Dictionary<string, SigilData>();
        }

        public SigilData GetRandomSigil()
        {
            if (Count == 0) return null;

            var allSigils = acctiveSigils.Concat(passiveSigils).ToDictionary(x => x.Key, x => x.Value);

            var sigilData = GetWeight(allSigils);

            if (sigilData != null) return sigilData;
            return null;
        }

        public SigilData GetRandomActiveSigil()
        {
            if (acctiveSigils == null || acctiveSigils.Count == 0) return null;

            var sigilData = GetWeight(acctiveSigils);

            if (sigilData != null) return sigilData;
            return null;
        }

        public SigilData GetRandomPassiveSigil()
        {
            if (passiveSigils == null || passiveSigils.Count == 0) return null;

            var sigilData = GetWeight(passiveSigils);

            if (sigilData != null) return sigilData;
            return null;
        }

        private SigilData GetWeight(Dictionary<string, SigilData> sigils)
        {
            float totalWeight = 0;

            foreach (var sigil in sigils)
                totalWeight += sigil.Value.Weight;

            float random = UnityEngine.Random.Range(0, totalWeight);
            float current = 0;

            foreach (var w in sigils)
            {
                current += w.Value.Weight;
                if (random < current)
                    return w.Value;
            }

            return null;
        }
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