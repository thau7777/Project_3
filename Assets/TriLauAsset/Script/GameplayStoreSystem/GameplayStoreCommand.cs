using UnityEngine;

namespace MyRule.CommandPattern
{
    public class GameplayStoreCommand : ICommand
    {
        private StoreView _storeView;

        public GameplayStoreCommand(StoreView storeView)
        {
            _storeView = storeView;
        }

        public void Execute()
        {
            
        }

        public void Undo()
        {
            _storeView.Hide();    
        }
    }
}