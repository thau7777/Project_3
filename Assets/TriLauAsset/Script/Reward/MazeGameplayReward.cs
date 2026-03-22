using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MyRule
{
    public abstract class Reward
    {

    }

    [Serializable]
    public class MazeGameplayReward : Reward
    {
        [JsonProperty] private int runeAmount;
        [JsonProperty] private int sigilAmount;

        [JsonIgnore] public int RuneAmount => runeAmount;    
        [JsonIgnore] public int SigilAmount => sigilAmount;

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