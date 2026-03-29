using Cysharp.Threading.Tasks;
using MyRule.Event;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MyRule
{
    public class AchievementManager : PersistentSingleton<AchievementManager>, IGameData
    {
        [SerializeField] private List<AchievementConfig> configs;

        private Dictionary<string, AchievementData> _achievementDict = new();
        private Dictionary<AchievementType, List<AchievementConfig>> _configByType = new();

        private void OnEnable()
        {
            GameSystemManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            GameSystemManager.Instance.Unregister(this);
        }

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

        public AchievementConfig GetAchievementById(string id)
        {
            return configs.Find(x => x.id == id);
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
            //EventBus<NoctificationAchievementEvent>.Raise(new NoctificationAchievementEvent)
        }

        public void GiveReward(AchievementConfig config)
        {
            if (config.goldReward > 0)
            {
                LobbyManager.Instance.IncreaseGold(config.goldReward);
            }
            
            if (config.crystalReward > 0)
            {
                LobbyManager.Instance.IncreaseCrystal(config.crystalReward);
            }

            if (config.sigilReward != null)
            {
                SigilData sigilData = new SigilData(config.sigilReward.id, config.sigilReward.sigilType, config.sigilReward.name, config.sigilReward.baseDmg, config.sigilReward.manaCost, config.sigilReward.rarity, config.sigilReward.keyBinding);
                SigilCollectionManager.Instance.AddSigil(sigilData);
            }
        }

        private void UpdateUIAchiement()
        {
            List<AchievementData> achievementDatas = new List<AchievementData>(_achievementDict.Values);

            EventBus<UpdateAchievementEvent>.Raise(new UpdateAchievementEvent(achievementDatas));
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

            UpdateUIAchiement();

            return UniTask.CompletedTask;
        }

        public void SaveData(GameData data)
        {
            data.SetAchivements(new List<AchievementData>(_achievementDict.Values));
        }
    }
}