using System.Collections.Generic;

namespace MyRule.UI
{
    public static class UIStateMachine
    {
        private static readonly Stack<PanelType> stateStack = new Stack<PanelType>();

        public static PanelType Current => stateStack.Count > 0 ? stateStack.Peek() : PanelType.MainMenu;

        public static void Push(PanelType state)
        {
            if (Current != state)
                stateStack.Push(state);
        }

        public static PanelType Pop()
        {
            if (stateStack.Count > 1)
                return stateStack.Pop();

            return Current;
        }

        public static void Reset(PanelType baseState)
        {
            stateStack.Clear();
            stateStack.Push(baseState);
        }
    }
}
