using MyRule.Event;
using Newtonsoft.Json;
using System;

namespace MyRule
{
    [JsonObject]
    public class AchievementData
    {
        [JsonProperty] protected string _id;
        [JsonProperty] private int _progress;
        [JsonProperty] private int _required;
        [JsonProperty] protected bool _isUnlocked;
        [JsonProperty] private bool _hasReceiveRewards;

        [JsonIgnore] public string ID => _id;
        [JsonIgnore] public int Progress => _progress;
        [JsonIgnore] public int Required => _required;
        [JsonIgnore] public bool IsUnlocked => _isUnlocked;
        [JsonIgnore] public bool HasReceiveRewards => _hasReceiveRewards;

        [JsonConstructor]
        public AchievementData(string id, int required)
        {
            this._id = id;
            this._isUnlocked = false;
            this._hasReceiveRewards = false;
            this._progress = 0;
            this._required = required;
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

        public virtual void UpdateProgress(int value) 
        {
            if (_isUnlocked) return;

            _progress += value;

            if (_progress >= (int)_required)
            {
                _progress = (int)_required;
                UnlockAchievement();
            }
        }

        public float GetCurrentProgress() => (float)_progress/_required;
    }
}