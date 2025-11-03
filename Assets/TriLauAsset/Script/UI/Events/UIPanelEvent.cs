namespace MyRule.UI
{
    public struct SwitchPanelEvent : IEvent
    {
        public readonly PanelType Type;

        public SwitchPanelEvent(PanelType type)
        {
            Type = type;
        }
    }
}