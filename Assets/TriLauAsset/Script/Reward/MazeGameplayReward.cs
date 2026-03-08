using UnityEngine;

namespace MyRule
{
    public abstract class Reward
    {
        public abstract int GetReward();
    }

    public class MazeGameplayReward : Reward
    {
        private int runeAmound;

        public MazeGameplayReward(int runeAmount)
        {
            this.runeAmound = runeAmount;
        }

        public override int GetReward() => runeAmound;
    }
}