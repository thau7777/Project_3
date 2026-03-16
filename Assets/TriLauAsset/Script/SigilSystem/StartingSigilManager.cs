using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class StartingSigilManager : Singleton<StartingSigilManager>, IGameData
    {
        [SerializeField] private Transform[] spawnPoints;

        private bool hasRecived = false;
        private bool isShowing = false;

        private List<Card> cardObjs = new List<Card>();

        private EventBinding<SigilChosenEvent> evtBinhding;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);

            evtBinhding = new EventBinding<SigilChosenEvent>(HideStartingSigil);
            EventBus<SigilChosenEvent>.Register(evtBinhding);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);

            EventBus<SigilChosenEvent>.Deregister(evtBinhding);
        }

        private void ShowStartingSigil()
        {
            if (isShowing) return;

            SpawnStartingSigilCardAsync().Forget();

            VolumeController.Instance.AdjustUIVolumeWeight();

            RTSCameraController.Instance.CanInteract = false;

            CardTracker.Instance.canInteract = true;
            CardTracker.Instance.isReward = true;

            isShowing = true;
        }

        private async void HideStartingSigil()
        {
            if (!isShowing) return;
            
            hasRecived = true;

            MatchManager.Instance.MatchData.SetHasRecivedStartingSigil(true);

            CardTracker.Instance.canInteract = false;

            await UniTask.Delay(100);

            VolumeController.Instance.AdjustUIVolumeWeight();

            await HideAllSigil();

            DesTroyCard();

            isShowing = false;

            RTSCameraController.Instance.CanInteract = true;
        }

        private async UniTask SpawnStartingSigilCardAsync()
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                SigilData sigilData = MatchManager.Instance.GetRandomSigilInMatch();

                if (sigilData == null) break;

                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id); ;

                if (sigilSO == null) continue;

                var cardObj = Instantiate(sigilSO.sigilPreb, spawnPoints[i]);
                Card card = cardObj.GetComponent<Card>();
                cardObjs.Add(card);
                card.SetSigil(sigilData, sigilSO);
                card.transform.DOMoveY(-2000f, 0.2f).SetEase(Ease.Linear);
                await UniTask.Delay(200);
                card.IsShowing = true;
            }
        }

        private SigilSO GetSigilFromCollection(SigilData sigilData)
        {
            while (true)
            {
                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);

                var existSigil = cardObjs.Find(s => s.SigilSO.id == sigilSO.id);

                if (existSigil == null) return sigilSO;
            }
        }

        private async UniTask HideAllSigil()
        {
            foreach (var card in cardObjs)
            {
                card.IsShowing = false;
                await UniTask.Delay(400);
                card.transform.DOMoveY(-2200.4f, 0.2f).SetEase(Ease.Linear);
            }
        }

        private void DesTroyCard()
        {
            foreach (var card in cardObjs)
            {
                Destroy(card.gameObject);
            }

            cardObjs.Clear();
        }

        public async UniTask LoadData(GameData data)
        {
            await UniTask.WaitUntil(() => MatchManager.Instance.MatchData != null);

            hasRecived = data.MatchData.HasReceivedStartingSigil;
            if (!hasRecived) ShowStartingSigil();
        }

        public void SaveData(GameData data)
        {
            //data.MatchData.SetHasRecivedStartingSigil(hasRecived);
        }
    }
}