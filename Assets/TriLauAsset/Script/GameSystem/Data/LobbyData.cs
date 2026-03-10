using Newtonsoft.Json;
using System;

namespace MyRule
{
    [Serializable]
    public class LobbyData
    {
        [JsonProperty] private int _gold;
        [JsonProperty] private int _crystal;

        [JsonIgnore] public int Gold => _gold;
        [JsonIgnore] public int Crystal => _crystal;

        public LobbyData()
        {
            this._gold = 0;
            this._crystal = 0;
        }

        public void SetGold(int gold) => this._gold = gold;
        public void SetCrystal(int crystal) => this._crystal = crystal;
    }
}