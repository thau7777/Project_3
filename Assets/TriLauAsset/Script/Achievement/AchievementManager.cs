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

                switch (config.type)
                {
                    case AchievementType.KillEnemy:
                        {
                            if (!_achievementDict.ContainsKey(config.id))
                            {
                                _achievementDict[config.id] = new KillEnemyAchievementData(config.id, false, 0, config.targetValue);
                            }
                            break;
                        }
                    case AchievementType.Discovery:
                        {
                            if (!_achievementDict.ContainsKey(config.id))
                            {
                                _achievementDict[config.id] = new DiscoveryAchievementData(config.id, false, config.targetMap);
                            }
                            break;
                        }
                    case AchievementType.CollectSigil:
                        {
                            if (!_achievementDict.ContainsKey(config.id))
                            {
                                _achievementDict[config.id] = new CollectSigilAchievementData(config.id, false, config.targetSigil.id);
                            }
                            break;
                        }
                    case AchievementType.Basic:
                        {
                            if (!_achievementDict.ContainsKey(config.id))
                            {
                                _achievementDict[config.id] = new AchievementData(config.id, false);
                            }
                            break;
                        }
                }
            }
        }

        public AchievementConfig GetAchievementById(string id)
        {
            return configs.Find(x => x.id == id);
        }

        public void Trigger(AchievementType type, object value)
        {
            if (!_configByType.ContainsKey(type)) return;

            foreach (var config in _configByType[type])
            {
                if (!_achievementDict.TryGetValue(config.id, out var data)) continue;

                if (data.IsUnlocked) continue;

                data.UpdateProgress(value);
            }
        }

        private void UnlockAchievement(AchievementConfig config, AchievementData data)
        {
            Debug.Log($"Unlocked Achievement: {config.id}");

            //GiveReward(config);

            // UI
            //EventBus<NoctificationAchievementEvent>.Raise(new NoctificationAchievementEvent(config));
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