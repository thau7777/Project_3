using Cysharp.Threading.Tasks;
using MyRule.Event;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class SigilStorageManager : PersistentSingleton<SigilStorageManager>
    {
        public SigilStorageSO sigilStorageSO;

        private EventBinding<SigilChosenEvent> sigilChosenEventBinding;

        private void OnEnable()
        {
            sigilChosenEventBinding = new EventBinding<SigilChosenEvent>(OnSigilChosen);
            EventBus<SigilChosenEvent>.Register(sigilChosenEventBinding);
        }

        private void OnDisable()
        {
            EventBus<SigilChosenEvent>.Deregister(sigilChosenEventBinding);
        }

        private void OnSigilChosen(SigilChosenEvent evt)
        {
            SigilSO sigilSO = sigilStorageSO.activeSigils.Find(s => s.activeSigilType == evt.normalSigilSO.activeSigilType);

            if (sigilSO != null) sigilStorageSO.activeSigils.Remove(sigilSO);

            sigilStorageSO.activeSigils.Add(evt.normalSigilSO);
            EventBus<AddSigilEnvet>.Raise(new AddSigilEnvet(evt.normalSigilSO));
            CharacterStatsManager.Instance.UpdateSigilStats(evt.normalSigilSO);
        }

        public void ResetSorage()
        {
            sigilStorageSO.activeSigils.Clear();
        }
    }
}