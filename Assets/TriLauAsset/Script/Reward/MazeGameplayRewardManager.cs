using UnityEngine;

namespace MyRule
{
    public class MazeGameplayRewardManager : PersistentSingleton<MazeGameplayRewardManager>
    {
        private MazeGameplayReward reward;

        private bool hasRewards = false;

        public bool HasRewards => hasRewards;

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
    }
}