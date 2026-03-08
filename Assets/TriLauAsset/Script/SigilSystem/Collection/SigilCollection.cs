using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyRule
{
    public class SigilCollection
    {
        [JsonProperty] private Dictionary<string, Sigil> sigils = new Dictionary<string, Sigil>();

        public int Count => sigils.Count;

        public Dictionary<string, Sigil> Sigils => sigils;

        public SigilCollection() { }

        public void AddSigil(Sigil sigil)
        {
            if (!sigils.ContainsKey(sigil.Id))
                sigils.Add(sigil.Id, sigil);
        }

        public void RemoveSigil(Sigil sigil) => sigils.Remove(sigil.Id);

        public Sigil GetRandomSigil()
        {
            if (sigils.Count == 0) return null;

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
    public class Sigil
    {
        [JsonProperty] private string id;
        [JsonProperty] private string name;
        [JsonProperty] private int rarity;

        public string Id => id;
        public string Name => name;
        public int Rarity => rarity;

        public Sigil(string name, int rarity)
        {
            this.id = Guid.NewGuid().ToString();
            this.name = name;
            this.rarity = rarity;
        }
    }
}