using Cysharp.Threading.Tasks;
using MyRule.Event;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class MatchManager : PersistentSingleton<MatchManager>, IGameData
    {
        private MatchData _matchData;

        public MatchData MatchData => _matchData;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        private void Update()
        {
            if (_matchData != null)
            {
                _matchData.IncreaseTimePlayed(Time.deltaTime);
            }
        }

        public bool IsNewMatch() => _matchData.IsNewMatch == true;

        public void CreateNewMatch(EMap mapType, CharacterData characterStatsData, int runeAmount, List<SigilData> sigilsInMatch)
        {
            _matchData = GameSystemManager.Instance.GameData.CreateNewMatch(mapType, characterStatsData, runeAmount);
            _matchData.SigilPool.CreatePool(sigilsInMatch);
        }

        public void FinishMatch()
        {
            HistoryManager.Instance.AddMatchToHistory(_matchData);
            GameSystemManager.Instance.GameData.SetMatch(null);
            _matchData = null;
        }

        public UniTask LoadData(GameData data)
        {
            if (data.MatchData == null) return UniTask.CompletedTask;

            _matchData = data.MatchData;
            EventBus<UpdateMatchResultEvent>.Raise(new UpdateMatchResultEvent(data.MatchData.Result));

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData == null) return;

            data.SetMatch(_matchData);
        }

        public UniTask NewGame()
        {
            _matchData = null;
            return UniTask.CompletedTask;
        }
    }
}