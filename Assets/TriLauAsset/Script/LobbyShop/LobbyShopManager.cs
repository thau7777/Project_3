using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class LobbyShopManager : MonoBehaviour, IGameData
    {
        [SerializeField] private GroupSigil baseLobbyShopSigils;

        private LobbyShopData shopData;

        public LobbyShopData ShopData => shopData;

        private void CreateNewShop()
        {
            shopData = new LobbyShopData();

            foreach (var sigil in baseLobbyShopSigils.sigilSOs)
            {
                SigilData sigilData = new SigilData(sigil.id, sigil.sigilType, sigil.name, sigil.phys, sigil.manaCost, sigil.rarity, sigil.keyBinding);
                shopData.LobbyShopSigils.Add(sigil.id, sigilData);
            }
        }

        public bool BuySigil(string id)
        {
            if (shopData.LobbyShopSigils.ContainsKey(id))
            {
                shopData.LobbyShopSigils.Remove(id);
                return true;
            }
            return false;
        }

        public UniTask LoadData(GameData data)
        {
            if (data.LobbyData.Shop.LobbyShopSigils != null && data.LobbyData.Shop.LobbyShopSigils.Count > 0)
            {
                shopData = data.LobbyData.Shop;
            }
            else
            {
                CreateNewShop();
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            data.LobbyData.SetShop(shopData);
        }
    }
}