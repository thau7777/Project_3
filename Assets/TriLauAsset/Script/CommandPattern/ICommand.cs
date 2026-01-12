using UnityEngine;


namespace MyRule.CommandPattern
{
    public interface ICommand
    {
        public void Execute();
        public void Undo();
    }
}