namespace MyRule.UI
{
    public struct AnyButtonPressEvent : IEvent { }

    public struct SubmitPressEvent : IEvent 
    {
        public readonly ButtonType ButtonType;
        public SubmitPressEvent(ButtonType buttonType)
        {
            ButtonType = buttonType;
        }
    }
    
    public struct CancelPressEvent : IEvent { }

    public struct MovePressEvent : IEvent
    {
        public readonly float Horizontal;
        public readonly float Vertical;

        public MovePressEvent(float horizontal, float vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }
    }

    public struct AdjustPressEvent : IEvent
    {
        public readonly float Horizontal;

        public AdjustPressEvent(float horizontal)
        {
            Horizontal = horizontal;
        }
    }
}