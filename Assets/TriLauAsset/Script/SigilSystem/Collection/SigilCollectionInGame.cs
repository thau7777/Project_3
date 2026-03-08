using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;

namespace MyRule
{
    public class SigilCollectionInGame : Singleton<SigilCollectionInGame>
    {
        [SerializeField] protected GroupSigil baseSigil;

        protected SigilCollection sigilCollection;

        const string keySave = "SigilCollectionInGame";

        private async void Start()
        {
            //Load();
            await UniTask.Delay(1000);
            CreateNewSigilCollection();
        }

        public SigilSO GetRandomSigil()
        {
            if (sigilCollection == null || sigilCollection.Count == 0) return null;

            Sigil sigil = sigilCollection.GetRandomSigil();

            if (sigil == null) return null;

            SigilSO sigilSO = baseSigil.sigilSOs.FirstOrDefault(s => s.name.Equals(sigil.Name));

            Debug.Log("Get random " + sigilSO.name);

            return sigilSO;
        }

        public void RemoveSigil(Sigil sigil)
        {
            sigilCollection.RemoveSigil(sigil);
        }

        public void ResetFromPlayerCollection()
        {
            sigilCollection = new SigilCollection();

            SigilCollection baseCollection = SigilCollectionManager.Instance.GetSigilCollection();

            foreach (var sigil in baseCollection.Sigils.Values)
            {
                Sigil newSigil = new Sigil(sigil.Name, sigil.Rarity);
                sigilCollection.AddSigil(newSigil);
            }

            //Save();
        }

        protected void CreateNewSigilCollection()
        {
            sigilCollection = new SigilCollection();

            SigilCollection baseSigilCollection = SigilCollectionManager.Instance.GetSigilCollection();

            foreach (var sigil in baseSigilCollection.Sigils.Values)
            {
                Sigil newSigil = new Sigil(sigil.Name, sigil.Rarity);
                sigilCollection.AddSigil(newSigil);
            }
        }

        //void Save()
        //{
        //    if (sigilCollection == null) return;

        //    string json = JsonUtility.ToJson(sigilCollection);

        //    PlayerPrefs.SetString(keySave, json);
        //    PlayerPrefs.Save();
        //}

        //void Load()
        //{
        //    if (PlayerPrefs.HasKey(keySave))
        //    {
        //        string json = PlayerPrefs.GetString(keySave);
        //        sigilCollection = JsonUtility.FromJson<SigilCollection>(json);
        //    }
        //    else
        //    {
        //        ResetFromPlayerCollection();
        //    }
        //}

        private void OnApplicationQuit()
        {
            //Save();
        }
    }
}