using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class MazeGameplayRewardManager : PersistentSingleton<MazeGameplayRewardManager>, IGameData
    {
        private MazeGameplayReward reward;

        private bool hasRewards = false;

        public bool HasRewards => hasRewards;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        public void CreateNewReward(int runeAmount, int sigil = 1)
        {
            if (hasRewards) return;

            reward = new MazeGameplayReward(runeAmount, sigil);
            hasRewards = true;
        }

        public MazeGameplayReward GetReward()
        {
            if (!hasRewards) return null;

            hasRewards = false;

            MazeGameplayReward recivedReward = reward;
            reward = null;

            return recivedReward;
        }

        public UniTask LoadData(GameData data)
        {
            if (data.MatchData.Reward != null)
            {
                reward = data.MatchData.Reward;
                hasRewards = true;
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (reward == null) return;
            data.MatchData.SetReward(reward);
        }
    }
}