using MyRule.Event;
using Newtonsoft.Json;
using System;
using static UnityEngine.Rendering.STP;

namespace MyRule
{
    [JsonObject]
    public class AchievementData
    {
        [JsonProperty] protected string _id;
        [JsonProperty] protected bool _isUnlocked;
        [JsonProperty] private bool _hasReceiveRewards;

        [JsonIgnore] public string ID => _id;
        [JsonIgnore] public bool IsUnlocked => _isUnlocked;
        [JsonIgnore] public bool HasReceiveRewards => _hasReceiveRewards;

        [JsonConstructor]
        public AchievementData(string id, bool isUnlocked)
        {
            this._id = id;
            this._isUnlocked = isUnlocked;
            this._hasReceiveRewards = false;
        }

        protected void UnlockAchievement()
        {
            _isUnlocked = true;

            EventBus<NoctificationAchievementEvent>.Raise(new NoctificationAchievementEvent(this));
        }

        public void ReceiveRewards()
        {
            if (!_isUnlocked || _hasReceiveRewards) return;
            _hasReceiveRewards = true;
        }

        public virtual void UpdateProgress(object data) 
        {
            if (data is bool value)
            {
                if (value)
                {
                    UnlockAchievement();
                }
            }
        }

        public virtual float GetCurrentProgress() => IsUnlocked? 1f : 0f;
    }

    [Serializable]
    public class KillEnemyAchievementData : AchievementData
    {
        [JsonProperty] private int _progress;
        [JsonProperty] private int _required;
        [JsonIgnore] public int Progress => _progress;
        [JsonIgnore] public int Required => _required;

        public KillEnemyAchievementData(string id, bool isUnlocked, int progress, int required) : base(id, isUnlocked)
        {
            this._progress = progress;
            this._required = required;
        }

        public override void UpdateProgress(object data)
        {
            if (data is int value)
            {
                IncreaseProgress(value);
            }
        }

        private void IncreaseProgress(int value)
        {
            if (_isUnlocked) return;

            _progress += value;

            if (_progress >= _required)
            {
                _progress = _required;
                UnlockAchievement();
            }
        }

        public override float GetCurrentProgress()
        {
            return (float)_progress / _required;
        }
    }

    [Serializable]
    public class DiscoveryAchievementData : AchievementData
    {
        [JsonProperty] private EMap _mapRequired;

        [JsonIgnore] public EMap MapTypeRequired => _mapRequired;

        public DiscoveryAchievementData(string id, bool isUnlocked, EMap mapReqired) : base(id, isUnlocked)
        {
            this._mapRequired = mapReqired;
        }

        public override void UpdateProgress(object data)
        {
            if (data is EMap mapType)
            {
                if (mapType == _mapRequired)
                {
                    UnlockAchievement();
                }
            }
        }
    }

    [Serializable]
    public class CollectSigilAchievementData : AchievementData
    {
        [JsonProperty] private string _sigilIdRequired;

        [JsonIgnore] public string SigilIdRequired => _sigilIdRequired;

        public CollectSigilAchievementData(string id, bool isUnlocked, string sigilIdRequired) : base(id, isUnlocked)
        {
            this._sigilIdRequired = sigilIdRequired;
        }

        public override void UpdateProgress(object data)
        {
            if (data is string sigilId)
            {
                if (sigilId == _sigilIdRequired)
                {
                    UnlockAchievement();
                }
            }
        }
    }
}