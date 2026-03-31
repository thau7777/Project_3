using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyRule
{
    [JsonObject]
    public class SigilCollectionData
    {
        [JsonProperty] protected Dictionary<string, SigilData> acctiveSigils;
        [JsonProperty] protected Dictionary<string, SigilData> passiveSigils;

        [JsonIgnore] public int Count => acctiveSigils.Count + passiveSigils.Count;

        [JsonIgnore] public Dictionary<string, SigilData> ActiveSigils => acctiveSigils;
        [JsonIgnore] public Dictionary<string, SigilData> PassiveSigils => passiveSigils;

        [JsonConstructor]
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
}