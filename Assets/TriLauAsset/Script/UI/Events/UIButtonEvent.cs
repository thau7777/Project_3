namespace MyRule.UI
{
    public struct SelectButtonEvent : IEvent
    {
        public readonly ButtonView Button;

        public SelectButtonEvent(ButtonView button)
        {
            Button = button;
        }
    }
}