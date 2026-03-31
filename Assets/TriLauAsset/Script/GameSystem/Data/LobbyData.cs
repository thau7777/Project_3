using Newtonsoft.Json;
using System;

namespace MyRule
{
    [JsonObject]
    public class LobbyData
    {
        [JsonProperty] private int _gold;
        [JsonProperty] private int _crystal;
        [JsonProperty] private LobbyShopData _shop;

        [JsonIgnore] public int Gold => _gold;
        [JsonIgnore] public int Crystal => _crystal;
        [JsonIgnore] public LobbyShopData Shop => _shop;

        [JsonConstructor]
        public LobbyData()
        {
            this._gold = 0;
            this._crystal = 0;
            this._shop = new LobbyShopData();
        }

        public void SetGold(int gold) => this._gold = gold;
        public void SetCrystal(int crystal) => this._crystal = crystal;
        public void SetShop(LobbyShopData shop) => this._shop = shop;
    }
}