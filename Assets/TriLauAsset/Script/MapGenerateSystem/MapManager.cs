using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

namespace MyRule
{
    public class MapManager : MonoBehaviour
    {
        public MapConfig config;
        public MapView view;

        public Map CurrentMap { get; private set; }

        private async void Start()
        {
            await UniTask.WaitUntil(() => MatchManager.Instance != null);

            if (MatchManager.Instance.IsNewMatch())
            {
                GenerateNewMap();
                MatchManager.Instance.MatchData.SetIsNewMatch(false);
            }
            else
            {
                if (PlayerPrefs.HasKey("Map"))
                {
                    string mapJson = PlayerPrefs.GetString("Map");
                    Map map = JsonConvert.DeserializeObject<Map>(mapJson);
                    // using this instead of .Contains()
                    if (map.path.Any(p => p.Equals(map.GetBossNode().point)))
                    {
                        GenerateNewMap();
                    }
                    else
                    {
                        CurrentMap = map;
                        view.ShowMap(map);
                    }
                }
                else
                {
                    GenerateNewMap();
                }
            }
        }

        public void GenerateNewMap()
        {
            Map map = MapGenerator.GetMap(config);
            CurrentMap = map;
            Debug.Log(map.ToJson());
            view.ShowMap(map);
        }

        public void SaveMap()
        {
            if (CurrentMap == null) return;

            string json = JsonConvert.SerializeObject(CurrentMap, Formatting.Indented,
                new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            PlayerPrefs.SetString("Map", json);
            PlayerPrefs.Save();
        }

        private void OnApplicationQuit()
        {
            SaveMap();
        }
    }
}
