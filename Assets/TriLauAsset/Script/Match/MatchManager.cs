using Cysharp.Threading.Tasks;
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

        public bool IsNewMatch() => _matchData.IsNewMatch;

        public void CreateNewMatch(EMap mapType, CharacterData characterStatsData, int runeAmount, SigilsInMatchData sigilsInMatch)
        {
            _matchData = GameSystemManager.Instance.GameData.CreateNewMatch(mapType, characterStatsData, runeAmount, sigilsInMatch);
        }

        public void FinishMatch() => _matchData = null;

        public SigilData GetRandomSigilInMatch()
        {
            SigilData sigilData = _matchData.SigilsInMatch.GetRandomSigil();

            return sigilData;
        }

        public SigilData GetRandomActiveSigilInMatch()
        {
            SigilData sigilData = _matchData.SigilsInMatch.GetRandomActiveSigil();

            return sigilData;
        }

        public SigilData GetRandomPassiveSigilInMatch()
        {
            SigilData sigilData = _matchData.SigilsInMatch.GetRandomPassiveSigil();

            return sigilData;
        }

        public void RemoveSigilInMatch(SigilData sigilData) => _matchData.SigilsInMatch.RemoveSigil(sigilData);

        public UniTask LoadData(GameData data)
        {
            if (data.MatchData != null)
            {
                _matchData = data.MatchData;
            }
            else
            {
                _matchData = null;
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            data.SetMatch(_matchData);
        }
    }
}