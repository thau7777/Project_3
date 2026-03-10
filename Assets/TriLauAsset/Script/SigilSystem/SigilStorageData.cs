using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MyRule
{
    [Serializable]
    public class SigilStorageData
    {
        [JsonProperty] private List<SigilData> _sigils;

        [JsonIgnore] public List<SigilData> Sigils => _sigils;

        public SigilStorageData()
        {
            _sigils = new List<SigilData>();
        }

        public void AddSigil(SigilData sigil) => _sigils.Add(sigil);
    }
}