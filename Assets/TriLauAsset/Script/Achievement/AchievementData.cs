using Newtonsoft.Json;
using System;

namespace MyRule
{
    [Serializable]
    public class AchievementData
    {
        [JsonProperty] private string _id;
        [JsonProperty] private bool _isUnlocked;
        [JsonProperty] private int _progress;

        [JsonIgnore] public string ID => _id;
        [JsonIgnore] public bool IsUnlocked => _isUnlocked;
        [JsonIgnore] public int Progress => _progress;

        public AchievementData(string id, bool isUnlocked, int progress) 
        {
            this._id = id;
            this._isUnlocked = isUnlocked;
            this._progress = progress;
        }

        public void UnlockAchievement() => _isUnlocked = true;

        public void IncreaseProgress(int value) => this._progress += value;
    }
}