using System.Collections.Generic;
using UnityEngine;

namespace MyRule.Event
{
    public struct UpdateLobbyShopSigilEvent : IEvent
    {
        public readonly List<LobbyShopProductData> sigilDatas;

        public UpdateLobbyShopSigilEvent(List<LobbyShopProductData> sigilDatas)
        {
            this.sigilDatas = sigilDatas; 
        }
    }

    public struct UpdateLobbyShopCardEvent : IEvent
    {
        public readonly List<LobbyShopProductData> cardDatas;
        
        public UpdateLobbyShopCardEvent(List<LobbyShopProductData> cardDatas)
        {
            this.cardDatas = cardDatas;
        }
    }
}