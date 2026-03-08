using MyRule.Event;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class SigilStorageManager : MonoBehaviour
    {
        public SigilStorageSO sigilStorageSO;

        private SigilSO sigil_L;
        private SigilSO sigil_R;
        private SigilSO sigil_S;
        private SigilSO sigil_F;
        private int index = 0;
        private List<SigilSO> passiveSigils;


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

        private void Awake()
        {
            passiveSigils = new List<SigilSO>();
        }

        private void OnSigilChosen(SigilChosenEvent evt)
        {
            sigilStorageSO.activeSigils.Add(evt.normalSigilSO);
            EventBus<AddSigilEnvet>.Raise(new AddSigilEnvet(evt.normalSigilSO));
            CharacterStatsManager.Instance.UpdateSigilStats(evt.normalSigilSO);
        }

        void Load()
        {
            foreach (var sigil in sigilStorageSO.activeSigils)
            {
                if (sigil != null)
                {
                    EventBus<AddSigilEnvet>.Raise(new AddSigilEnvet(sigil));
                }
            }
        }
    }
}