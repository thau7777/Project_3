using System.Collections.Generic;
using UnityEngine;

namespace MyRule.CommandPattern
{
    public static class CommandInvoker
    {
        private const int MAX_STACK_SIZE = 10;

        private static Stack<ICommand> _undoStack = new Stack<ICommand>();
        private static Stack<ICommand> _redoStack = new Stack<ICommand>();

        public static void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);

            _redoStack.Clear();
        }

        public static void UndoCommand()
        {
            if (_undoStack.Count > 0)
            {
                ICommand activeCommand = _undoStack.Pop();
                _redoStack.Push(activeCommand);
                activeCommand.Undo();
            }
        }

        public static void RedoCommand()
        {
            if ( _redoStack.Count > 0)
            {
                ICommand activeCommand = _redoStack.Pop();
                _undoStack.Push(activeCommand);
                activeCommand.Execute();
            }
        }

        private static void PushWithLimit(Stack<ICommand> stack, ICommand command)
        {
            stack.Push(command);

            if (stack.Count <= MAX_STACK_SIZE)
                return;

            ICommand[] temp = stack.ToArray();
            stack.Clear();

            for (int i = temp.Length - 2; i >= 0; i--)
            {
                stack.Push(temp[i]);
            }
        }
    }
}