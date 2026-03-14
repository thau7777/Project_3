using Cysharp.Threading.Tasks;
using MyRule.Event;
using MyRule.UI;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyRule
{
    public class SigilStorageManager : PersistentSingleton<SigilStorageManager>, IGameData
    {
        private SigilStorageData sigilStorageData;

        public SigilStorageData SigilStorageData => sigilStorageData;

        private EventBinding<SigilChosenEvent> sigilChosenEventBinding;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);

            sigilChosenEventBinding = new EventBinding<SigilChosenEvent>(OnSigilChosen);
            EventBus<SigilChosenEvent>.Register(sigilChosenEventBinding);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);

            EventBus<SigilChosenEvent>.Deregister(sigilChosenEventBinding);
        }

        private void OnSigilChosen(SigilChosenEvent evt)
        {
            SigilData sigilData = new SigilData(evt.sigilSO.id, evt.sigilSO.sigilType, evt.sigilSO.name, evt.sigilSO.rarity);

            string id = sigilData.Id;

            var existActive = sigilStorageData.ActiveSigils.ContainsKey(id);
            if (existActive) sigilStorageData.ActiveSigils.Remove(id);

            var existPassive = sigilStorageData.PassiveSigils.ContainsKey(id);
            if (existPassive) sigilStorageData.PassiveSigils.Remove(id);

            if (evt.sigilSO.sigilType == SigilType.Active)
                sigilStorageData.ActiveSigils.Add(sigilData.Id, sigilData);
            else if (evt.sigilSO.sigilType == SigilType.Passive)
                sigilStorageData.PassiveSigils.Add(sigilData.Id, sigilData);

            EventBus<AddSigilEvent>.Raise(new AddSigilEvent(evt.sigilSO));
            CharacterManager.Instance.UpdateSigilStats(evt.sigilSO);
        }

        #region Save Load
        public UniTask LoadData(GameData data)
        {
            sigilStorageData = new SigilStorageData();

            if (data.MatchData != null)
            {
                if (data.MatchData.SigilStorageInMatch != null)
                {
                    sigilStorageData = data.MatchData.SigilStorageInMatch;

                    foreach (var sigil in sigilStorageData.ActiveSigils)
                    {
                        SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigil.Key);

                        if (sigilSO != null)
                        {
                            EventBus<AddSigilEvent>.Raise(new AddSigilEvent(sigilSO));
                        }
                    }
                }
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData != null)
            {         
                data.MatchData.SetSigilStorageInMatch(sigilStorageData);
            }
        }
        #endregion
    }
}