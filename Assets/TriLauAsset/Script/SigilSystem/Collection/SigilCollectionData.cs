using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyRule
{
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
    }

    [Serializable]
    public class SigilsInMatchData : SigilCollectionData
    {
        public SigilsInMatchData()
        {
            acctiveSigils = new Dictionary<string, SigilData>();
            passiveSigils = new Dictionary<string, SigilData>();
        }

        public void SetActiveSigils(Dictionary<string, SigilData> sigils) => acctiveSigils = sigils;

        public void SetPassiveSigils(Dictionary<string, SigilData> sigils) => passiveSigils = sigils;

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
            int totalWeight = 0;

            foreach (var w in sigils.Values)
                totalWeight += w.Rarity;

            int random = UnityEngine.Random.Range(0, totalWeight);
            int current = 0;

            foreach (var w in sigils.Values)
            {
                current += w.Rarity;
                if (random < current)
                    return w;
            }

            return null;
        }
    }

    [Serializable]
    public class SigilData
    {
        [JsonProperty] private string id;
        [JsonProperty] private string name;
        [JsonProperty] private SigilType sigilType;
        [JsonProperty] private int rarity;

        [JsonIgnore] public string Id => id;
        [JsonIgnore] public string Name => name;
        [JsonIgnore] public SigilType SigilType => sigilType;
        [JsonIgnore] public int Rarity => rarity;

        public SigilData(string id, SigilType sigilType, string name, int rarity)
        {
            this.id = id;
            this.name = name;
            this.sigilType = sigilType;
            this.rarity = rarity;
        }
    }
}