using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    [Serializable]
    public class CardData
    {
        [JsonProperty] private string _id;

        [JsonIgnore] public string ID => _id;

        public CardData(string id) 
        {
            _id = id;
        }
    }

    [Serializable] 
    public class LobbyShopProductData
    {
        [JsonProperty] private string _id;
        [JsonProperty] private bool _ísSold;
        [JsonIgnore] public string ID => _id;
        [JsonIgnore] public bool IsSold => _ísSold;

        public LobbyShopProductData(string id)
        {
            _id = id;
            _ísSold = false;
        }
    }

    [Serializable]
    public class LobbyShopData
    {
        [JsonProperty] private Dictionary<EProduct, List<LobbyShopProductData>> _lobbyShopProducts;

        [JsonIgnore] public Dictionary<EProduct, List<LobbyShopProductData>> LobbyShopProducts => _lobbyShopProducts;

        public LobbyShopData() 
        {
            _lobbyShopProducts = new Dictionary<EProduct, List<LobbyShopProductData>>();    
        }

        public void CreateNewListProduct(EProduct product) => _lobbyShopProducts[product] = new List<LobbyShopProductData>();

        public bool ConstainProduct(EProduct eProduct, LobbyShopProductData data) => _lobbyShopProducts[eProduct].Contains(data);

        public bool ConstainProduct(EProduct eProduct, string id)
        {
            var existProduct = _lobbyShopProducts[eProduct].Find(x => x.ID == id);

            if (existProduct != null)
            {
                return true;
            }

            return false;
        }

        public void RemoveProduct(EProduct eProduct, string id)
        {
            var existProduct = _lobbyShopProducts[eProduct].Find(x => x.ID == id);

            if (existProduct != null)
            {
                _lobbyShopProducts[eProduct].Remove(existProduct);
            }
        }
    }
}