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

        private CancellationTokenSource goldCts;
        private CancellationTokenSource crystalCts;

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

        private async void UpdateGoldText(UpdateLobbyGoldUIEvent evt)
        {
            goldCts?.Cancel();
            goldCts?.Dispose();
            goldCts = new CancellationTokenSource();

            int oldGold = gold;
            int newGold = evt.value;

            await AnimateValue(goldTxt, oldGold, newGold, 0.5f, goldCts.Token);

            gold = newGold;
        }

        private async void UpdateCrystalText(UpdateLobbyCrystalUIEvent evt)
        {
            crystalCts?.Cancel();
            crystalCts?.Dispose();
            crystalCts = new CancellationTokenSource();

            int oldCrystal = crystal;
            int newCrystal = evt.value;

            await AnimateValue(crystalTxt, oldCrystal, newCrystal, 0.5f, crystalCts.Token);

            crystal = newCrystal;
        }

        private async UniTask AnimateValue(TextMeshProUGUI txt, int from, int to, float duration, CancellationToken token)
        {
            float time = 0f;

            while (time < duration)
            {
                if (token.IsCancellationRequested) return;

                time += Time.deltaTime;
                float t = time / duration;

                int value = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
                txt.text = value.ToString();

                await UniTask.Yield();
            }

            txt.text = to.ToString(); 
        }
    }
}