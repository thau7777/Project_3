using Cysharp.Threading.Tasks;
using MyRule.Event;
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
            SigilData sigilData = new SigilData(evt.sigilSO.id, evt.sigilSO.sigilType, evt.sigilSO.name, evt.sigilSO.mag, evt.sigilSO.manaCost, evt.sigilSO.rarity, evt.sigilSO.keyBinding);

            if (sigilData.SigilType == SigilType.Active)
            {
                if (!sigilStorageData.IsActiveSigilFull())
                {
                    int index = sigilStorageData.GetFirstEmptySlotActive();

                    sigilStorageData.TryAddActiveSigil(index, sigilData);

                    EventBus<AddActiveSigilEvent>.Raise(new AddActiveSigilEvent(index, evt.sigilSO));
                }
                else return;
            }
            else if (sigilData.SigilType == SigilType.Passive)
            {
                if (!sigilStorageData.IsPassiveSigilFull())
                {
                    int index = sigilStorageData.GetFirstEmptySlotPassive();

                    sigilStorageData.TryAddPassiveSigil(index, sigilData);

                    EventBus<AddPassiveSigilEvent>.Raise(new AddPassiveSigilEvent(index, evt.sigilSO));
                }
                else return;
            }

            CharacterManager.Instance.UpdateSigilStats(evt.sigilSO);
        }

        public SigilSO GetRandomActiveSigilInStorage() => GetRandomSigil(sigilStorageData.ActiveSigils);

        public SigilSO GetRandomPassiveSigilInStorage() => GetRandomSigil(sigilStorageData.PassiveSigils);

        private SigilSO GetRandomSigil(SigilData[] sigilDatas)
        {
            int i = UnityEngine.Random.Range(0, sigilDatas.Length);
            SigilData sigilData = sigilDatas[i];
            if (sigilData != null)
            {
                return SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);
            }

            return null;
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

                EventBus<AddActiveSigilEvent>.Raise(new AddActiveSigilEvent(i, sigilSO));
            }

            foreach (var (sigilData, i) in sigilStorageData.PassiveSigils.Select((s, i) => (s, i)))
            {
                if (sigilData == null) continue;
                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);
                if (sigilSO == null) continue;

                EventBus<AddPassiveSigilEvent>.Raise(new AddPassiveSigilEvent(i, sigilSO));
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