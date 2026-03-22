using MyRule.Event;
using System;
using UnityEngine;

namespace MyRule
{
    public enum CombatType
    {
        EnemyFighting,
        BossFigihting,
    }

    [Serializable]
    public class CombatData
    {
        public CombatType combatType;
        public EMatchResult result;
        
        public CombatData(CombatType combatType)
        {
            result = EMatchResult.None;
            this.combatType = combatType;
        }

        public void SetResult(EMatchResult result) => this.result = result;
    }

    public class CombatManager : PersistentSingleton<CombatManager>
    {
        private CombatData combatResult;

        public CombatType combatType => combatResult.combatType;
        public EMatchResult Result => combatResult.result;

        private void CreateNewCombat(CombatType combatType) => combatResult = new CombatData(combatType);

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
            
            CreateNewCombat(CombatType.EnemyFighting);

            EventBus<UpdateTDCombatWavesEvent>.Raise(new UpdateTDCombatWavesEvent(tdWaves));
            EventBus<UpdateTBCombatWavesEvent>.Raise(new UpdateTBCombatWavesEvent(tbWaves));
            EventBus<ShowCombatChoiceEvent>.Raise(new ShowCombatChoiceEvent(true));
        }

        public void CreateBossFighting()
        {
            GroupWave tdWaves = WaveManager.Instance.CreateNewWave();
            GroupWave tbWaves = WaveManager.Instance.CreateNewWave();

            CreateNewCombat(CombatType.BossFigihting);

            EventBus<UpdateTDCombatWavesEvent>.Raise(new UpdateTDCombatWavesEvent(tdWaves));
            EventBus<UpdateTBCombatWavesEvent>.Raise(new UpdateTBCombatWavesEvent(tbWaves));
            EventBus<ShowCombatChoiceEvent>.Raise(new ShowCombatChoiceEvent(true));
        }
    }
}