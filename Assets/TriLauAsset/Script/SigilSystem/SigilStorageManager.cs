using Cysharp.Threading.Tasks;
using MyRule.Event;
using System;
using System.Linq;

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
            AddSigilToStorage(evt.sigilSO, evt.index);
        }

        public void AddSigilToStorage(SigilSO sigilSO, int slotIndex = -1)
        {
            SigilData sigilData = new SigilData(sigilSO.id, sigilSO.sigilType, sigilSO.name, sigilSO.mag, sigilSO.manaCost, sigilSO.rarity, sigilSO.keyBinding);

            if (sigilData.SigilType == SigilType.Active)
            {
                int index = 0;

                if (!sigilStorageData.IsActiveSigilFull())
                {
                    index = sigilStorageData.GetFirstEmptySlotActive();
                }
                else if (slotIndex != -1)
                {
                    index = slotIndex;
                }
                else if (slotIndex == -1 && sigilStorageData.IsActiveSigilFull())
                {
                    index = UnityEngine.Random.Range(0, sigilStorageData.ActiveSigils.Length);
                }

                sigilStorageData.TryAddActiveSigil(index, sigilData);

                EventBus<AddActiveSigilEvent>.Raise(new AddActiveSigilEvent(index, sigilSO, sigilData));
            }
            else if (sigilData.SigilType == SigilType.Passive)
            {
                int index = 0;
                if (!sigilStorageData.IsPassiveSigilFull())
                {
                    index = sigilStorageData.GetFirstEmptySlotPassive();
                }
                else if (slotIndex != -1)
                {
                    index = slotIndex;
                }
                else if (slotIndex == -1 && sigilStorageData.IsPassiveSigilFull())
                {
                    index = UnityEngine.Random.Range(0, sigilStorageData.PassiveSigils.Length);
                }

                sigilStorageData.TryAddPassiveSigil(index, sigilData);

                EventBus<AddPassiveSigilEvent>.Raise(new AddPassiveSigilEvent(index, sigilSO, sigilData));
            }

            CharacterManager.Instance.UpdateSigilStats(sigilSO);

            switch (sigilSO.sigilName)
            {
                case "Blood Fang":
                    EventBus<UpdateInkDialogueVariableEvent>.Raise(new UpdateInkDialogueVariableEvent("hasBloodFang", true));
                    break;
            }
        }

        public SigilStorageSlotData GetRandomActiveSigilInStorage() => GetRandomSigil(sigilStorageData.ActiveSigils);

        public SigilStorageSlotData GetRandomPassiveSigilInStorage() => GetRandomSigil(sigilStorageData.PassiveSigils);

        private SigilStorageSlotData GetRandomSigil(SigilData[] sigilDatas)
        {
            int i = UnityEngine.Random.Range(0, sigilDatas.Length);
            SigilData sigilData = sigilDatas[i];
            if (sigilData != null)
            {
                return new SigilStorageSlotData(i, sigilData);
            }

            return null;
        }

        public void RemoveSigil(int index, SigilSO sigilSO)
        {
            if (sigilSO.sigilType == SigilType.Active)
            {
                sigilStorageData.RemoveActiveSigil(index);
                EventBus<RemoveActiveSigilEvent>.Raise(new RemoveActiveSigilEvent(index));
            }
            else if (sigilSO.sigilType == SigilType.Passive)
            {
                sigilStorageData.RemovePassiveSigil(index);
                EventBus<RemovePassiveSigilEvent>.Raise(new RemovePassiveSigilEvent(index));
            }    
        }    

        #region Save Load
        public UniTask LoadData(GameData data)
        {
            sigilStorageData = new SigilStorageData();

            if (data.MatchData?.SigilStorageInMatch == null)
                return UniTask.CompletedTask;

            sigilStorageData = data.MatchData.SigilStorageInMatch;

            foreach (var (sigilData, i) in sigilStorageData.ActiveSigils.Select((s, i) => (s, i)))
            {
                if (sigilData == null) continue;
                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);
                if (sigilSO == null) continue;

                EventBus<AddActiveSigilEvent>.Raise(new AddActiveSigilEvent(i, sigilSO, sigilData));
            }

            foreach (var (sigilData, i) in sigilStorageData.PassiveSigils.Select((s, i) => (s, i)))
            {
                if (sigilData == null) continue;
                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);
                if (sigilSO == null) continue;

                EventBus<AddPassiveSigilEvent>.Raise(new AddPassiveSigilEvent(i, sigilSO, sigilData));
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