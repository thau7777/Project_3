using Cysharp.Threading.Tasks;
using MyRule.Event;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class SigilStorageManager : PersistentSingleton<SigilStorageManager>, IGameData
    {
        public SigilStorageSO sigilStorageSO;

        private EventBinding<SigilChosenEvent> sigilChosenEventBinding;

        private void OnEnable()
        {
            GameSystemManager.instance.Register(this);

            sigilChosenEventBinding = new EventBinding<SigilChosenEvent>(OnSigilChosen);
            EventBus<SigilChosenEvent>.Register(sigilChosenEventBinding);
        }

        private void OnDisable()
        {
            GameSystemManager.instance?.Unregister(this);

            EventBus<SigilChosenEvent>.Deregister(sigilChosenEventBinding);
        }

        private void OnSigilChosen(SigilChosenEvent evt)
        {
            SigilSO sigilSO = sigilStorageSO.activeSigils.Find(s => s.keyBinding == evt.normalSigilSO.keyBinding);

            if (sigilSO != null) sigilStorageSO.activeSigils.Remove(sigilSO);

            sigilStorageSO.activeSigils.Add(evt.normalSigilSO);
            EventBus<AddSigilEnvet>.Raise(new AddSigilEnvet(evt.normalSigilSO));
            CharacterStatsManager.Instance.UpdateSigilStats(evt.normalSigilSO);
        }

        public void ResetSorage()
        {
            sigilStorageSO.activeSigils.Clear();
        }

        #region Save Load
        public UniTask LoadData(GameData data)
        {
            if (data.MatchData != null)
            {
                sigilStorageSO.activeSigils = new List<SigilSO>();

                foreach (var sigil in data.MatchData.SigilsInMatch.ActiveSigils)
                {

                }
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData != null)
            {

            }
        }
        #endregion
    }
}