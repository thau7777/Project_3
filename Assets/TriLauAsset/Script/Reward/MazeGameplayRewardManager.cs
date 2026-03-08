using UnityEngine;

namespace MyRule
{
    public class MazeGameplayRewardManager : PersistentSingleton<MazeGameplayRewardManager>
    {
        private MazeGameplayReward reward;

        private bool hasRewards = false;

        public bool HasRewards => hasRewards;

        public void CreateNewReward(int runeAmount)
        {
            if (hasRewards) return;

            reward = new MazeGameplayReward(runeAmount);
            hasRewards = true;
        }

        public MazeGameplayReward GetReward() => reward;
    }
}