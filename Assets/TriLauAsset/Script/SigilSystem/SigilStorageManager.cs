using Cysharp.Threading.Tasks;
using MyRule.Event;
using MyRule.UI;
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
            var existActive = sigilStorageSO.activeSigils.Find(s => s.keyBinding == evt.sigilSO.keyBinding);
            if (existActive != null) sigilStorageSO.activeSigils.Remove(existActive);

            var existPassive = sigilStorageSO.passiveSigils.Find(s => s.keyBinding == evt.sigilSO.keyBinding);
            if (existPassive != null) sigilStorageSO.passiveSigils.Remove(existPassive);

            if (evt.sigilSO.sigilType == SigilType.Active)
                sigilStorageSO.activeSigils.Add(evt.sigilSO);
            else if (evt.sigilSO.sigilType == SigilType.Passive)
                sigilStorageSO.passiveSigils.Add(evt.sigilSO);

            EventBus<AddSigilEvent>.Raise(new AddSigilEvent(evt.sigilSO));
            CharacterStatsManager.Instance.UpdateSigilStats(evt.sigilSO);
        }

        public void ResetStorage()
        {
            sigilStorageSO.activeSigils.Clear();
            sigilStorageSO.passiveSigils.Clear();
        }

        #region Save Load
        public async UniTask LoadData(GameData data)
        {
            sigilStorageSO.activeSigils = new List<SigilSO>();
            sigilStorageSO.passiveSigils = new List<SigilSO>();

            await UniTask.NextFrame();

            if (data.MatchData != null)
            {
                if (data.MatchData.SigilStorageInMatch != null)
                {
                    if (data.MatchData.SigilStorageInMatch.ActiveSigils != null && data.MatchData.SigilStorageInMatch.ActiveSigils.Count != 0)
                    {
                        foreach (var sigil in data.MatchData.SigilStorageInMatch.ActiveSigils)
                        {
                            SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigil.Key);
                            sigilStorageSO.activeSigils.Add(sigilSO);
                            //EventBus<AddSigilEvent>.Raise(new AddSigilEvent(sigilSO));
                        }
                    }

                    if (data.MatchData.SigilStorageInMatch.PassiveSigils != null && data.MatchData.SigilStorageInMatch.PassiveSigils.Count != 0)
                    {
                        foreach (var sigil in data.MatchData.SigilStorageInMatch.PassiveSigils)
                        {
                            SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigil.Key);
                            sigilStorageSO.passiveSigils.Add(sigilSO);
                            //EventBus<AddSigilEvent>.Raise(new AddSigilEvent(sigilSO));
                        }
                    }
                }
            }
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData != null)
            {
                SigilStorageData sigilStorageData = new SigilStorageData();
                
                foreach (var sigilSO in sigilStorageSO.activeSigils)
                {
                    SigilData sigilData = new SigilData(sigilSO.id, sigilSO.sigilType, sigilSO.name, sigilSO.rarity);
                    sigilStorageData.AddSigil(sigilData);
                }

                foreach (var sigilSO in sigilStorageSO.passiveSigils)
                {
                    SigilData sigilData = new SigilData(sigilSO.id, sigilSO.sigilType, sigilSO.name, sigilSO.rarity);
                    sigilStorageData.AddSigil(sigilData);
                }

                data.MatchData.SetSigilStorageInMatch(sigilStorageData);
            }
        }
        #endregion
    }
}