using System;
using UnityEngine;

namespace MyRule
{
    [Serializable]
    public class CombatResult
    {
        public EMatchResult result;
        
        public CombatResult() 
        {
            result = EMatchResult.None;
        }

        public void SetResult(EMatchResult result) => this.result = result;
    }

    public class CombatManager : PersistentSingleton<CombatManager>
    {
        private CombatResult combatResult;

        public EMatchResult Result => combatResult.result;

        public void CreateNewCombat() => combatResult = new CombatResult();

        public void SetCombatResultWin()
        {
            combatResult.SetResult(EMatchResult.Win);
            MazeGameplayRewardManager.Instance.CreateNewReward(100);
        }

        public void SetCombatResultLose()
        {
            combatResult.SetResult(EMatchResult.Lose);
            MatchManager.Instance.MatchData.SetMatchResult(EMatchResult.Lose);
        }
    }
}