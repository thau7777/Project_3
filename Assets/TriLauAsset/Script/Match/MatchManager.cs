using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class MatchManager : PersistentSingleton<MatchManager>, IGameData
    {
        private MatchData _matchData;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        public void CreateNewMatch(int runeAmount, SigilsInMatchData sigilsInMatch)
        {
            GameSystemManager.instance.GameData.CreatNewMatch(runeAmount, sigilsInMatch);
        }

        public void FinishMatch() => _matchData = null;

        public SigilSO GetRandomSigilInMatch()
        {
            SigilData sigilData = _matchData.SigilsInMatch.GetRandomSigil();

            if (sigilData != null)
            {
                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);

                return sigilSO;
            }

            return null;
        }

        public SigilSO GetRandomActiveSigilInMatch()
        {
            SigilData sigilData = _matchData.SigilsInMatch.GetRandomActiveSigil();

            if (sigilData != null)
            {
                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);

                return sigilSO;
            }

            return null;
        }

        public SigilSO GetRandomPassiveSigilInMatch()
        {
            SigilData sigilData = _matchData.SigilsInMatch.GetRandomPassiveSigil();

            if (sigilData != null)
            {
                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(sigilData.Id);

                return sigilSO;
            }

            return null;
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
            if (_matchData != null)
            {
                data.SetMatch(_matchData);
            }
            else
            {
                data?.SetMatch(null);
            }
        }
    }
}