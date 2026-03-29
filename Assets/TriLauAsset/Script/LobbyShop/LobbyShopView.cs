using MyRule.Event;
using UnityEngine;

namespace MyRule.UI
{
    public class LobbyShopView : MonoBehaviour
    {
        [SerializeField] private LobbyShopProductView[] sigilProducts;
        [SerializeField] private LobbyShopProductView[] cardProducts;
        [SerializeField] private LobbyShopProductView[] goldProducts;
        [SerializeField] private LobbyShopProductView[] crystalProducts;

        private EventBinding<UpdateLobbyShopSigilEvent> updateLobbyShopSigilEvent;
        private EventBinding<UpdateLobbyShopCardEvent> updateLobbyShopCardEvent;

        private void OnEnable()
        {
            updateLobbyShopSigilEvent = new EventBinding<UpdateLobbyShopSigilEvent>(HandleSigils);
            EventBus<UpdateLobbyShopSigilEvent>.Register(updateLobbyShopSigilEvent);

            updateLobbyShopCardEvent = new EventBinding<UpdateLobbyShopCardEvent>(HandleCards);
            EventBus<UpdateLobbyShopCardEvent>.Register(updateLobbyShopCardEvent);
        }

        private void OnDisable()
        {
            EventBus<UpdateLobbyShopSigilEvent>.Deregister(updateLobbyShopSigilEvent);
            EventBus<UpdateLobbyShopCardEvent>.Deregister(updateLobbyShopCardEvent);
        }

        private void Start()
        {
            SetBaseGoldAndCrystal();
        }

        private void HandleSigils(UpdateLobbyShopSigilEvent evt)
        {
            for (int i = 0; i < sigilProducts.Length && i < evt.sigilDatas.Count; i++)
            {
                if (sigilProducts[i] != null)
                {
                    LobbyShopProductConfig productConfic = LobbyShopManager.Instance.GetProductConfic(evt.sigilDatas[i].ID);
                    sigilProducts[i].SetProduct(productConfic);
                }
            }
        }    

        private void HandleCards(UpdateLobbyShopCardEvent evt)
        {
            for (int i = 0; i < cardProducts.Length && i < evt.cardDatas.Count; i++)
            {
                if (cardProducts[i] != null)
                {
                    LobbyShopProductConfig productConfic = LobbyShopManager.Instance.GetProductConfic(evt.cardDatas[i].ID);
                    cardProducts[i].SetProduct(productConfic);
                }
            }
        }

        private void SetBaseGoldAndCrystal()
        {
            for (int i = 0; i < goldProducts.Length; i++)
            {
                goldProducts[i].SetProduct();
            }

            for (int i = 0; i < crystalProducts.Length; i++)
            {
                crystalProducts[i].SetProduct();
            }
        }
    }
}