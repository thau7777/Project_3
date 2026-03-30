using System.Collections.Generic;
using UnityEngine;

namespace MyRule.Event
{
    public struct UpdateAchievementEvent : IEvent
    {
        public readonly List<AchievementData> achievementDatas;

        public UpdateAchievementEvent(List<AchievementData> achievementDatas)
        {
            this.achievementDatas = achievementDatas;
        }
    }

    public struct NoctificationAchievementEvent : IEvent
    {
        public readonly AchievementData achievement;

        public NoctificationAchievementEvent(AchievementData achievement)
        {
            this.achievement = achievement;
        }
    }
}