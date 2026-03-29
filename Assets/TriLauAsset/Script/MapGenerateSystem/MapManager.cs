using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

namespace MyRule
{
    public class MapManager : Singleton<MapManager>, IGameData
    {
        [SerializeField] private MapConfig config;
        [SerializeField] private MapView view;

        public Map CurrentMap { get; private set; }

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        public void GenerateNewMap()
        {
            Map map = MapGenerator.GetMap(config);
            CurrentMap = map;
            view.ShowMap(map);
        }

        public async UniTask LoadData(GameData data)
        {
            await UniTask.WaitUntil(() => MatchManager.Instance != null);

            if (MatchManager.Instance.IsNewMatch())
            {
                GenerateNewMap();
                MatchManager.Instance.MatchData.SetIsNewMatch(false);
            }
            else
            {
                Map map = data.MatchData.Map;

                if (map != null)
                {
                    if (map.Path.Any(p => p == map.GetBossNode().Point))
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

        public void SaveData(GameData data)
        {
            if (CurrentMap == null) return;

            if (data.MatchData != null)
            {
                data.MatchData.SetMap(CurrentMap);
            }
        }
    }
}
