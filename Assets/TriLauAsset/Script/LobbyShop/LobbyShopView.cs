using Cysharp.Threading.Tasks;
using MyRule.Event;
using System.Threading;
using TMPro;
using UnityEngine;

namespace MyRule.UI
{
    public class LobbyShopView : MonoBehaviour
    {
        [SerializeField] private LobbyShopProductView[] sigilProducts;
        [SerializeField] private LobbyShopProductView[] cardProducts;
        [SerializeField] private LobbyShopProductView[] goldProducts;
        [SerializeField] private LobbyShopProductView[] crystalProducts;
        [SerializeField] private TextMeshProUGUI goldTxt;
        [SerializeField] private TextMeshProUGUI crystalTxt;

        private int gold = 0;
        private int crystal = 0;

        private EventBinding<UpdateLobbyShopSigilEvent> updateLobbyShopSigilEvent;
        private EventBinding<UpdateLobbyShopCardEvent> updateLobbyShopCardEvent;
        private EventBinding<UpdateLobbyGoldUIEvent> updateLobbyGoldUIEvent;
        private EventBinding<UpdateLobbyCrystalUIEvent> updateLobbyCrystalUIEvent;

        private CancellationTokenSource cts;

        private void OnEnable()
        {
            updateLobbyShopSigilEvent = new EventBinding<UpdateLobbyShopSigilEvent>(HandleSigils);
            EventBus<UpdateLobbyShopSigilEvent>.Register(updateLobbyShopSigilEvent);

            updateLobbyShopCardEvent = new EventBinding<UpdateLobbyShopCardEvent>(HandleCards);
            EventBus<UpdateLobbyShopCardEvent>.Register(updateLobbyShopCardEvent);

            updateLobbyGoldUIEvent = new EventBinding<UpdateLobbyGoldUIEvent>(UpdateGoldText);
            EventBus<UpdateLobbyGoldUIEvent>.Register(updateLobbyGoldUIEvent);

            updateLobbyCrystalUIEvent = new EventBinding<UpdateLobbyCrystalUIEvent>(UpdateCrystalText);
            EventBus<UpdateLobbyCrystalUIEvent>.Register(updateLobbyCrystalUIEvent);
        }

        private void OnDisable()
        {
            EventBus<UpdateLobbyShopSigilEvent>.Deregister(updateLobbyShopSigilEvent);
            EventBus<UpdateLobbyShopCardEvent>.Deregister(updateLobbyShopCardEvent);
            EventBus<UpdateLobbyGoldUIEvent>.Deregister(updateLobbyGoldUIEvent);
            EventBus<UpdateLobbyCrystalUIEvent>.Deregister(updateLobbyCrystalUIEvent);
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

        private void UpdateGoldText(UpdateLobbyGoldUIEvent evt)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            Transition.TransitionValue(
                setter: value => goldTxt.text = value.ToString(),
                from: gold,
                to: evt.value,
                duration: 0.2f,
                token: cts.Token).Forget();

            gold = evt.value;
        }

        private void UpdateCrystalText(UpdateLobbyCrystalUIEvent evt)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            Transition.TransitionValue(
                setter: value => crystalTxt.text = value.ToString(),
                from: crystal,
                to: evt.value,
                duration: 0.2f,
                token: cts.Token).Forget();

            crystal = evt.value;
        }
    }
}