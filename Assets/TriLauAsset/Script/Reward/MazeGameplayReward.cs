using UnityEngine;

namespace MyRule
{
    public abstract class Reward
    {
    }

    public class MazeGameplayReward : Reward
    {
        private int runeAmount;
        private int sigilAmount;

        public int RuneAmount => runeAmount;    
        public int SigilAmount => sigilAmount;

        public MazeGameplayReward()
        {
            runeAmount = 0;
            sigilAmount = 0;
        }

        public MazeGameplayReward(int runeAmount, int sigilAmount)
        {
            this.runeAmount = runeAmount;
            this.sigilAmount = sigilAmount;
        }
    }
}