using MyRule.Event;
using UnityEngine;

namespace MyRule.UI
{
    public class HUDPresenter
    {
        IBaseUIView baseUIView;
        private SigilView[] sigils;
        private ItemView[] items;

        private EventBinding<AddSigilEvent> sigilEventBinding;
        private EventBinding<AddItemEvent> itemEventBinding;
        private EventBinding<HUDEvent> hudEventBinding;

        public HUDPresenter(IBaseUIView baseUIView, SigilView[] sigilViews, ItemView[] itemViews)
        {
            this.baseUIView = baseUIView;

            this.sigils = sigilViews; 
            this.items = itemViews;

            sigilEventBinding = new EventBinding<AddSigilEvent>(HandleSigil);
            EventBus<AddSigilEvent>.Register(sigilEventBinding);
            itemEventBinding = new EventBinding<AddItemEvent>(HandleItem);
            EventBus<AddItemEvent>.Register(itemEventBinding);
            hudEventBinding = new EventBinding<HUDEvent>(HandleHUDEvent);
            EventBus<HUDEvent>.Register(hudEventBinding);
        }

        public void Clearup()
        {
            EventBus<AddSigilEvent>.Deregister(sigilEventBinding);
            EventBus<AddItemEvent>.Deregister(itemEventBinding);
            EventBus<HUDEvent>.Deregister(hudEventBinding);
        }

        private void HandleSigil(AddSigilEvent evt)
        {
            for (int i = 0; i < sigils.Length; i++)
            {
                if (sigils[i].Key == evt.sigilSO.keyBinding)
                {
                    sigils[i].SetSigil(evt.sigilSO);
                }
            }
        }

        private void HandleItem(AddItemEvent evt)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].IsEmpty)
                {
                    items[i].SetIcon(evt.item);
                    return;
                }
            }
        }

        private void HandleHUDEvent()
        {

        }
    }
}