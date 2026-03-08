using MyRule.Event;
using UnityEngine;

namespace MyRule
{
    public class StorePresenter
    {
        private IStoreView _storeView;

        private EventBinding<OpenStoreEvent> _openStoreEventBinding;

        public StorePresenter(IStoreView storeView)
        {
            _storeView = storeView;

            _openStoreEventBinding = new EventBinding<OpenStoreEvent>(HandleOpenStore);
            EventBus<OpenStoreEvent>.Register(_openStoreEventBinding);
        }

        public void Clearup()
        {
            EventBus<OpenStoreEvent>.Deregister(_openStoreEventBinding);
        }

        private void HandleOpenStore()
        {

        }
    }
}