using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace MyRule
{
    public class AchievementManager : PersistentSingleton<AchievementManager>, IGameData
    {
        [SerializeField] private List<AchievementConfig> configs;

        private Dictionary<string, AchievementData> _achievementDict = new();
        private Dictionary<AchievementType, List<AchievementConfig>> _configByType = new();

        private void SetConfig()
        {
            foreach (var config in configs)
            {
                if (!_configByType.ContainsKey(config.type))
                    _configByType[config.type] = new List<AchievementConfig>();

                _configByType[config.type].Add(config);

                if (!_achievementDict.ContainsKey(config.id))
                {
                    _achievementDict[config.id] = new AchievementData(config.id, false, 0);
                }
            }
        }

        public void Trigger(AchievementType type, int value = 1)
        {
            if (!_configByType.ContainsKey(type)) return;

            foreach (var config in _configByType[type])
            {
                var data = _achievementDict[config.id];

                if (data.IsUnlocked) continue;

                data.IncreaseProgress(value);

                if (data.Progress >= config.targetValue)
                {
                    UnlockAchievement(config, data);
                }
            }
        }

        private void UnlockAchievement(AchievementConfig config, AchievementData data)
        {
            data.UnlockAchievement();

            Debug.Log($"Unlocked Achievement: {config.id}");

            //GiveReward(config);

            // UI
            // AchievementUI.Show(config);
        }

        private void GiveReward(AchievementConfig config)
        {
            if (config.goldReward > 0)
            {
                // Player.AddGold(config.goldReward);
            }

            if (!string.IsNullOrEmpty(config.sigilRewardId))
            {
                SigilSO sigilSO = SigilCollectionManager.Instance.GetSigilSOById(config.sigilRewardId);
                SigilData sigilData = new SigilData(sigilSO.id, sigilSO.sigilType, sigilSO.sigilName, sigilSO.phys, sigilSO.manaCost, sigilSO.rarity, sigilSO.keyBinding);
                SigilCollectionManager.Instance.AddSigil(sigilData);
            }
        }


        public UniTask LoadData(GameData data)
        {
            if (data.Achievements != null && data.Achievements.Count == 0)
            {
                SetConfig();
            }
            else if (data.Achievements != null && data.Achievements.Count > 0)
            {
                foreach (var achievement in data.Achievements)
                {
                    _achievementDict[achievement.ID] = achievement;
                }
            }

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            data.SetAchivements(new List<AchievementData>(_achievementDict.Values));
        }
    }
}