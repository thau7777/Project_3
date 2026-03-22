using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class LobbyShopData
    {
        [JsonProperty] private Dictionary<string, SigilData> lobbyShopSigils;

        [JsonIgnore] public Dictionary<string, SigilData> LobbyShopSigils => lobbyShopSigils;

        public LobbyShopData() 
        {
            lobbyShopSigils = new Dictionary<string, SigilData>();
        }
    }
}