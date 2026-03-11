using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MyRule
{
    [Serializable]
    public class SigilStorageData : SigilCollectionData
    {
        public SigilStorageData() 
        {
            acctiveSigils = new Dictionary<string, SigilData>();
            passiveSigils = new Dictionary<string, SigilData>();
        }
    }
}