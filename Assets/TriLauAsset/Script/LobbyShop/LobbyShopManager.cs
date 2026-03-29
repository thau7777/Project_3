using Cysharp.Threading.Tasks;
using MyRule.Event;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class LobbyShopManager : Singleton<LobbyShopManager>, IGameData
    {
        [SerializeField] private List<LobbyShopProductConfig> configs;

        private LobbyShopData shopData = new();
        private Dictionary<string, LobbyShopProductConfig> productsConfig = new();

        public LobbyShopData ShopData => shopData;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        private void SetConfig()
        {
            foreach (var config in configs)
            {
                if (!shopData.LobbyShopProducts.ContainsKey(config.productType))
                {
                    shopData.CreateNewListProduct(config.productType);
                }

                LobbyShopProductData lobbyShopProductData = new LobbyShopProductData(config.id);

                if (!shopData.ConstainProduct(config.productType, lobbyShopProductData))
                {
                    shopData.LobbyShopProducts[config.productType].Add(lobbyShopProductData);
                }

                if (!productsConfig.ContainsKey(config.id))
                {
                    productsConfig[config.id] = config;
                }
            }
        }

        public bool BuySigil(EUnit unit, int prices, LobbyShopProductConfig productConfic)
        {
            switch (unit)
            {
                case EUnit.Gold:
                    {
                        int currentGold = LobbyManager.Instance.CurrentGold;
                        if (currentGold >= prices)
                        {
                            SigilCollectionManager.Instance.AddSigil(productConfic.sigilSO);

                            LobbyManager.Instance.DecreaseGold(prices);

                            RemoveProduct(productConfic);

                            return true;
                        }
                        else return false;
                    }
                case EUnit.Crystal:
                    {
                        int currentCrystal = LobbyManager.Instance.CurrentCrystal;
                        if (currentCrystal >= prices)
                        {
                            SigilCollectionManager.Instance.AddSigil(productConfic.sigilSO);

                            LobbyManager.Instance.DecreaseCrystal(prices);

                            RemoveProduct(productConfic);

                            return true;
                        }
                        else return false;
                    }
            }
            return false;
        }

        public LobbyShopProductConfig GetProductConfic(string id) => productsConfig[id];

        public bool BuyCard(EUnit unit, int prices, LobbyShopProductConfig productConfic)
        {
            switch (unit)
            {
                case EUnit.Gold:
                    {
                        int currentGold = LobbyManager.Instance.CurrentGold;
                        if (currentGold >= prices)
                        {
                            //SigilCollectionManager.Instance.AddSigil(sigilSO);

                            LobbyManager.Instance.DecreaseGold(prices);

                            RemoveProduct(productConfic);

                            return true;
                        }
                        else return false;
                    }
                case EUnit.Crystal:
                    {
                        int currentCrystal = LobbyManager.Instance.CurrentCrystal;
                        if (currentCrystal >= prices)
                        {
                            //SigilCollectionManager.Instance.AddSigil(sigilSO);

                            LobbyManager.Instance.DecreaseCrystal(prices);

                            RemoveProduct(productConfic);

                            return true;
                        }
                        else return false;
                    }
            }
            return false;
        }

        public bool BuyGold(int prices, int gold)
        {
            int currentCrystal = LobbyManager.Instance.CurrentCrystal;

            if (currentCrystal >= prices)
            {
                LobbyManager.Instance.IncreaseGold(gold);
                LobbyManager.Instance.DecreaseCrystal(prices);
                return true;
            }
            return false;
        }   

        public bool BuyCrystal(int prices, int crystal)
        {
            LobbyManager.Instance.IncreaseCrystal(crystal);
            return true;
        }

        private void RemoveProduct(LobbyShopProductConfig productConfic)
        {
            if (shopData.ConstainProduct(productConfic.productType, productConfic.sigilSO.id))
            {
                shopData.RemoveProduct(productConfic.productType, productConfic.sigilSO.id);
            }
        }

        public UniTask LoadData(GameData data)
        {
            if (data.LobbyData.Shop != null && data.LobbyData.Shop.LobbyShopProducts.Count > 0)
            {
                shopData = data.LobbyData.Shop;
            }
            else
            {
                SetConfig();
            }

            EventBus<UpdateLobbyShopSigilEvent>.Raise(new UpdateLobbyShopSigilEvent(shopData.LobbyShopProducts[EProduct.Sigil]));
            EventBus<UpdateLobbyShopCardEvent>.Raise(new UpdateLobbyShopCardEvent(shopData.LobbyShopProducts[EProduct.Card]));

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            data.LobbyData.SetShop(shopData);
        }
    }
}