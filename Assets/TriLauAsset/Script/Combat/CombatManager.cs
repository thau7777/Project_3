using Cysharp.Threading.Tasks;
using MyRule.Event;
using Newtonsoft.Json;
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
        [JsonProperty] private CombatType combatType;
        [JsonProperty] private EMatchResult result;
        [JsonProperty] private GroupWave groupWave;
        [JsonProperty] private Loader.EScene scene;

        [JsonIgnore] public CombatType CombatType => combatType;
        [JsonIgnore] public EMatchResult Result => result;
        [JsonIgnore] public GroupWave GroupWave => groupWave;
        [JsonIgnore] public Loader.EScene Scene => scene;

        public CombatData(CombatType combatType)
        {
            result = EMatchResult.None;
            this.combatType = combatType;
            groupWave = null;
        }

        public void SetResult(EMatchResult result) => this.result = result;

        public void SetGroupWave(GroupWave groupWave) => this.groupWave = groupWave;

        public void SetScene(Loader.EScene scene) => this.scene = scene;
    }

    public class CombatManager : PersistentSingleton<CombatManager>, IGameData
    {
        private CombatData combatData;

        public CombatData CombatData => combatData;

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

        private void CreateNewCombat(CombatType combatType) => combatData = new CombatData(combatType);

        public void FinishCombat() => combatData = null;

        public void SetCombatResultWin()
        {
            combatData.SetResult(EMatchResult.Win);
            if (combatData.CombatType == CombatType.EnemyFighting)
            {
                MazeGameplayRewardManager.Instance.CreateNewReward(100);
            }
            else if (combatData.CombatType == CombatType.BossFigihting)
            {
                MazeGameplayRewardManager.Instance.CreateNewReward(100);
                MapTypeManager.Instance.MoveToNextMap();
            }
            combatData = null;
        }

        public void SetCombatResultLose()
        {
            combatData.SetResult(EMatchResult.Lose);
            MatchManager.Instance.MatchData.SetMatchResult(EMatchResult.Lose);
            combatData = null;
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

        public UniTask LoadData(GameData data)
        {
            if (data.MatchData == null) return UniTask.CompletedTask;

            if (data.MatchData.CombatData != null)
            {
                combatData = data.MatchData.CombatData;
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            if (data.MatchData == null) return;

            data.MatchData.SetCombat(combatData);
        }

        public UniTask NewGame()
        {
            combatData = null;

            return UniTask.CompletedTask;
        }
    }
}