namespace MyRule.Event
{
    public struct DialogueCamEvent : IEvent
    {
        public readonly string camName;

        public DialogueCamEvent(string camName)
        {
            this.camName = camName;
        }
    }
}