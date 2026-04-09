using Newtonsoft.Json;
using System;
using UnityEngine;

namespace MyRule
{
    [Serializable]
    public class MazeGameplayReward
    {
        [JsonProperty] private int runeAmount;
        [JsonProperty] private int sigilAmount;

        [JsonIgnore] public int RuneAmount => runeAmount;    
        [JsonIgnore] public int SigilAmount => sigilAmount;

        public MazeGameplayReward(int runeAmount, int sigilAmount)
        {
            this.runeAmount = runeAmount;
            this.sigilAmount = sigilAmount;
        }
    }
}