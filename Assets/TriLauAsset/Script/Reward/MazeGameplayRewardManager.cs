using Cysharp.Threading.Tasks;
using MyRule.Event;
using System.Threading.Tasks;
using UnityEngine;

namespace MyRule
{
    public class MazeGameplayRewardManager : PersistentSingleton<MazeGameplayRewardManager>, IGameData
    {
        private MazeGameplayReward reward;

        private bool hasRewards = false;

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

        public UniTask LoadData(GameData data)
        {
            if (data.MatchData != null && data.MatchData.Reward != null)
            {
                EventBus<MazeGameplayRewardEvent>.Raise(new MazeGameplayRewardEvent(reward));
                reward = null;
                hasRewards = false;
            }
            else
            {
                hasRewards = false;
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData != null)
            {
                data.MatchData.SetReward(reward);
            }
        }

        public UniTask NewGame()
        {
            reward = null;
            hasRewards = false;
            return UniTask.CompletedTask;
        }
    }
}