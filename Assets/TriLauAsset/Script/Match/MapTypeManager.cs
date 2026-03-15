using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class MapTypeManager : PersistentSingleton<MapTypeManager>, IGameData
    {
        private EMap currentMap;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        public EMap GetMapType() => currentMap;

        public void SetMapType(EMap mapType) => currentMap = mapType;

        public UniTask LoadData(GameData data)
        {
            currentMap = EMap.GreenLand;

            if (data.MatchData != null)
            {
                this.currentMap = data.MatchData.MapType;
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData != null)
            {
                data.MatchData.SetMap(this.currentMap);
            }
        }
    }
}