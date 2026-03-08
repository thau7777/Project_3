using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

namespace MyRule
{
    public class SigilCollectionManager : PersistentSingleton<SigilCollectionManager>
    {
        [SerializeField] protected GroupSigil baseSigil;
        [SerializeField] protected GroupSigil allSigil;
        [SerializeField] protected string keySave = "SigilCollection";

        protected SigilCollection sigilCollection;

        protected virtual void Start()
        {
            //Load();

            CreateNewSigilCollection();
        }

        public SigilCollection GetSigilCollection() => sigilCollection;

        public void AddSigil(Sigil sigil)
        {
            sigilCollection.AddSigil(sigil);
        }

        protected virtual void CreateNewSigilCollection()
        {
            sigilCollection = new SigilCollection();

            foreach (var sigilSO in baseSigil.sigilSOs)
            {
                Sigil sigil = new Sigil(sigilSO.name, sigilSO.rarity);
                sigilCollection.AddSigil(sigil);
            }
        }

        //protected virtual void Load() 
        //{
        //    if (PlayerPrefs.HasKey(keySave))
        //    {
        //        string sigilCollectionJson = PlayerPrefs.GetString(keySave);
        //        SigilCollection sigilCollection = JsonConvert.DeserializeObject<SigilCollection>(sigilCollectionJson);

        //        if (sigilCollection == null || sigilCollection.Count == 0)
        //        {
        //            CreateNewSigilCollection();
        //        }
        //        else
        //        {
        //            this.sigilCollection = sigilCollection;
        //        }
        //    }
        //    else
        //    {
        //        CreateNewSigilCollection();
        //    }
        //}

        //protected virtual void Save()
        //{
        //    if (sigilCollection == null) return;

        //    string json = JsonConvert.SerializeObject(sigilCollection, Formatting.Indented,
        //        new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        //    PlayerPrefs.SetString(keySave, json);
        //    PlayerPrefs.Save();
        //}

        private void OnApplicationQuit()
        {
            //Save();
        }
    }
}