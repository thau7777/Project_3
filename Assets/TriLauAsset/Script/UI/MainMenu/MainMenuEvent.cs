using MyRule.UI;

namespace MyRule
{
    public struct MainMenuButtonSelectedEvent : IEvent
    {
        public readonly ButtonType ButtonType { get; }

        public MainMenuButtonSelectedEvent(ButtonType buttonType)
        {
            ButtonType = buttonType;
        }
    }
}