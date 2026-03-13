using MyRule.Event;
using UnityEngine;

namespace MyRule.UI
{
    public class HUDPresenter
    {
        IHUDView hudView;

        private SigilView[] sigils;
        private ItemView[] items;

        private EventBinding<AddSigilEvent> sigilEventBinding;
        private EventBinding<AddItemEvent> itemEventBinding;
        private EventBinding<OpenHUDEvent> hudEventBinding;

        public HUDPresenter(IHUDView view, SigilView[] sigilViews, ItemView[] itemViews)
        {
            this.hudView = view;
            this.sigils = sigilViews; 
            this.items = itemViews;

            sigilEventBinding = new EventBinding<AddSigilEvent>(HandleSigil);
            EventBus<AddSigilEvent>.Register(sigilEventBinding);
            itemEventBinding = new EventBinding<AddItemEvent>(HandleItem);
            EventBus<AddItemEvent>.Register(itemEventBinding);
            hudEventBinding = new EventBinding<OpenHUDEvent>(HandleHUDEvent);
            EventBus<OpenHUDEvent>.Register(hudEventBinding);
        }

        public void Clearup()
        {
            EventBus<AddSigilEvent>.Deregister(sigilEventBinding);
            EventBus<AddItemEvent>.Deregister(itemEventBinding);
            EventBus<OpenHUDEvent>.Deregister(hudEventBinding);
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
            if (evt.index < 0 || evt.index >= items.Length) return;
            items[evt.index].SetIcon(evt.item);
        }

        private void HandleHUDEvent(OpenHUDEvent evt)
        {
            if (evt.show == true) hudView.ShowHUD();
            else if (evt.show == false) hudView.HideHUD();
        }
    }
}