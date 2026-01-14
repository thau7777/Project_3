using MyRule.CommandPattern;
using UnityEngine;

namespace MyRule
{
    public class PlanetCommand : ICommand
    {
        private PlanetManager planetManager;
        
        public PlanetCommand(PlanetManager planetManager)
        {
            this.planetManager = planetManager;
        }

        public void Execute()
        {
            if (planetManager.planetTargetd == null) return;
            planetManager.Interact();
        }

        public void Undo()
        {
            planetManager.Escape();
        }
    }
}