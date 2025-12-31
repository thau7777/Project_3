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

    public struct SwitchCamEvent : IEvent
    {
        public readonly int Cam;

        public SwitchCamEvent(int cam)
        {
            Cam = cam;
        }
    }
}