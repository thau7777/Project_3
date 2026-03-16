using MyRule.Event;
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

        private void CreateNewCombat() => combatResult = new CombatResult();

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

        public void CreateCombat()
        {
            GroupWave tdWaves = WaveManager.Instance.CreateNewWave();
            GroupWave tbWaves = WaveManager.Instance.CreateNewWave();
            
            CreateNewCombat();

            EventBus<UpdateTDCombatWavesEvent>.Raise(new UpdateTDCombatWavesEvent(tdWaves));
            EventBus<UpdateTBCombatWavesEvent>.Raise(new UpdateTBCombatWavesEvent(tbWaves));
            EventBus<ShowCombatChoiceEvent>.Raise(new ShowCombatChoiceEvent(true));
        }
    }
}