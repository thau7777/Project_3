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
            _configByType.Clear();
            _achievementDict.Clear();

            foreach (var config in configs)
            {
                if (!_configByType.ContainsKey(config.type))
                    _configByType[config.type] = new List<AchievementConfig>();

                _configByType[config.type].Add(config);

                if (!_achievementDict.ContainsKey(config.id))
                {
                    _achievementDict[config.id] = new AchievementData(config.id, config.targetValue);
                }
            }
        }

        public AchievementConfig GetAchievementById(string id)
        {
            return configs.Find(x => x.id == id);
        }

        public void Trigger<T>(AchievementType type, T value)
        {
            if (!_configByType.ContainsKey(type)) return;

            foreach (var config in _configByType[type])
            {
                if (!_achievementDict.TryGetValue(config.id, out var data)) continue;

                if (data.IsUnlocked) continue;

                if (value is int v1)
                {
                    data.UpdateProgress(v1);
                }
                else if (value is EMap v2)
                {
                    if (v2 != config.targetMap) continue;

                    data.UpdateProgress(1);
                }
                else if (value is string v3)
                {
                    if (v3 != config.targetSigil.id) continue;

                    data.UpdateProgress(1);
                }
            }
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
            SetConfig();

            if (data.Achievements != null && data.Achievements.Count > 0)
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

        public UniTask NewGame()
        {
            _achievementDict.Clear();

            return UniTask.CompletedTask;
        }
    }
}