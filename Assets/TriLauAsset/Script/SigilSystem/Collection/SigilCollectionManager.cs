using Cysharp.Threading.Tasks;
using MyRule.Event;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule
{
    public class SigilCollectionManager : PersistentSingleton<SigilCollectionManager>, IGameData
    {
        [SerializeField] protected GroupSigil baseSigil;
        [SerializeField] protected GroupSigil allSigil;

        private SigilCollectionData sigilCollection;

        private void OnEnable()
        {
            GameSystemManager.instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.instance.Unregister(this);
        }

        public SigilsInMatchData GetSigilCollection()
        {
            if (sigilCollection == null) return null;

            SigilsInMatchData sigilsInMatchData = new SigilsInMatchData();

            if (sigilCollection.ActiveSigils != null && sigilCollection.ActiveSigils.Count > 0)
            {
                var activeSigilList = sigilCollection.ActiveSigils.ToList();

                for (int i = activeSigilList.Count - 1; i > 0; i--)
                {
                    int randomIndex = UnityEngine.Random.Range(0, i + 1);
                    (activeSigilList[i], activeSigilList[randomIndex]) = (activeSigilList[randomIndex], activeSigilList[i]);
                }

                var result = activeSigilList.Take(activeSigilList.Count - 2).ToDictionary(x => x.Key, x => x.Value);

                sigilsInMatchData.SetActiveSigils(result);
            }

            if (sigilCollection.PassiveSigils != null && sigilCollection.PassiveSigils.Count > 0)
            {
                var passiveSigilList = sigilCollection.PassiveSigils.ToList();

                for (int i = passiveSigilList.Count - 1; i > 0; i--)
                {
                    int randomIndex = UnityEngine.Random.Range(0, i + 1);
                    (passiveSigilList[i], passiveSigilList[randomIndex]) = (passiveSigilList[randomIndex], passiveSigilList[i]);
                }

                var result = passiveSigilList.Take(passiveSigilList.Count - 2).ToDictionary(x => x.Key, x => x.Value);

                sigilsInMatchData.SetPassiveSigils(result);
            }

            return sigilsInMatchData;
        }

        public SigilSO GetSigilSOById(string id)
        {
            return allSigil.sigilSOs.Find(s => s.id == id);
        }

        public void AddSigil(SigilData sigil)
        {
            sigilCollection.AddSigil(sigil);
        }

        private void CreateNewSigilCollection()
        {
            sigilCollection = new SigilCollectionData();

            foreach (var sigilSO in baseSigil.sigilSOs)
            {
                SigilData sigil = new SigilData(sigilSO.id, sigilSO.sigilType, sigilSO.name, sigilSO.mag, sigilSO.manaCost, sigilSO.rarity, sigilSO.keyBinding);
                sigilCollection.AddSigil(sigil);
            }

            EventBus<UpdateSigilCollectionEvent>.Raise(new UpdateSigilCollectionEvent(sigilCollection));
        }

        public UniTask LoadData(GameData data)
        {
            if (data.SigilCollection == null || (data.SigilCollection != null && data.SigilCollection.Count == 0))
            {
                CreateNewSigilCollection();
            }
            else if (data.SigilCollection != null && data.SigilCollection.Count != 0)
            {
                sigilCollection = data.SigilCollection;
                EventBus<UpdateSigilCollectionEvent>.Raise(new UpdateSigilCollectionEvent(sigilCollection));
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            data.SetSigilCollection(sigilCollection);
        }
    }
}