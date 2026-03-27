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

        private List<Card> cards = new List<Card>();

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

            CardTracker.Instance.UnlockInteract(true);

            isShowing = true;
        }

        private async void HideStartingSigil()
        {
            if (!isShowing) return;
            
            hasRecived = true;

            MatchManager.Instance.MatchData.SetHasRecivedStartingSigil(true);

            CardTracker.Instance.UnlockInteract(false);

            await UniTask.Delay(100);

            VolumeController.Instance.AdjustUIVolumeWeight();

            await HideAllSigil();

            isShowing = false;

            RTSCameraController.Instance.CanInteract = true;
        }

        private async UniTask SpawnStartingSigilCardAsync()
        {
            List<SigilData> sigils = MatchManager.Instance.MatchData.SigilPool.GetActiveSigils(spawnPoints.Length);

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                SigilData sigilData = sigils[i];

                if (sigilData == null) break;

                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id); ;

                if (sigilSO == null) continue;

                Card card = CardPoolManager.Instance.Spawn(sigilSO.id);
                card.SetSigil(sigilData, sigilSO, CardGameplayType.Reward);
                card.transform.SetParent(spawnPoints[i]);
                card.transform.position = spawnPoints[i].position;
                card.transform.DOMoveY(-2000f, 0.2f).SetEase(Ease.Linear);
                cards.Add(card);
                await UniTask.Delay(200);
            }
        }

        private async UniTask HideAllSigil()
        {
            foreach (var card in cards)
            {
                card.ReleasePool();
                await UniTask.Delay(200);
                card.transform.DOMoveY(-2200.4f, 0.2f).SetEase(Ease.Linear);
            }
            
            cards.Clear();
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