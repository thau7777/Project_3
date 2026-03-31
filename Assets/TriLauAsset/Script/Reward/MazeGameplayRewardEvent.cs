namespace MyRule.Event
{
    public struct MazeGameplayRewardEvent : IEvent
    {
        public readonly MazeGameplayReward reward;

        public MazeGameplayRewardEvent(MazeGameplayReward reward)
        {
            this.reward = reward;
        }
    }
}