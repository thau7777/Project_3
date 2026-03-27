using UnityEngine;

namespace MyRule.CommandPattern
{
    public class CardDetailCommand : ICommand
    {
        private CardDetailView cardDetailView;

        public CardDetailCommand(CardDetailView cardDetailView)
        {
            this.cardDetailView = cardDetailView;
        }

        public void Execute()
        {
            cardDetailView.ShowDetail();
        }

        public void Undo()
        {
            cardDetailView.HideDetail();
        }
    }
}