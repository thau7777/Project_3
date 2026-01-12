using UnityEngine;
using MyRule.CommandPattern;

namespace MyRule
{
    public class HoloTableInteractCommand : ICommand
    {
        private HoloTable holoTable;

        public HoloTableInteractCommand(HoloTable holoTable)
        {
            this.holoTable = holoTable;
        }

        public void Execute() 
        {
            if (!holoTable.HasActive)
            {
                holoTable.Interact();
            }
        }

        public void Undo()
        {
            if (!holoTable.HasActive) return;

            holoTable.Exit();
        }
    }
}