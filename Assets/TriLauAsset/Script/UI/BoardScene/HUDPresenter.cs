using MyRule.Event;
using UnityEngine;

namespace MyRule.UI
{
    public class HUDPresenter
    {
        private IHUDView hudView;

        private InputReader inputReader;

        private SigilSlotView[] activeSigils;
        private SigilSlotView[] passiveSigils;
        private ItemView[] items;

        private bool isShowingStorage = true;
        private bool isShowPassiveSigil = false;

        private EventBinding<AddActiveSigilEvent> aSICEventBinding;
        private EventBinding<AddPassiveSigilEvent> pSEventBinding;
        private EventBinding<AddItemEvent> itemEventBinding;
        private EventBinding<OpenHUDEvent> hudEventBinding;

        private int activeSigilsInCombatLenght = 0;

        public HUDPresenter(IHUDView view, SigilSlotView[] activeSigil, SigilSlotView[] passiveSigils, ItemView[] itemViews, InputReader inputReader)
        {
            this.hudView = view;
            this.activeSigils = activeSigil;
            this.passiveSigils = passiveSigils;
            this.items = itemViews;
            this.inputReader = inputReader;

            aSICEventBinding = new EventBinding<AddActiveSigilEvent>(HandleASIC);
            EventBus<AddActiveSigilEvent>.Register(aSICEventBinding);

            pSEventBinding = new EventBinding<AddPassiveSigilEvent>(HandlePS);
            EventBus<AddPassiveSigilEvent>.Register(pSEventBinding);

            itemEventBinding = new EventBinding<AddItemEvent>(HandleItem);
            EventBus<AddItemEvent>.Register(itemEventBinding);
            
            hudEventBinding = new EventBinding<OpenHUDEvent>(HandleHUDEvent);
            EventBus<OpenHUDEvent>.Register(hudEventBinding);
            
            inputReader.diceRollActions.onOpenSigilStorage += HandleSigilStorage;
            inputReader.diceRollActions.onOpenPassiveSigilStorage += HandlePassiveSigilStorage;
        }

        public void Clearup()
        {
            EventBus<AddActiveSigilEvent>.Deregister(aSICEventBinding);
            EventBus<AddPassiveSigilEvent>.Deregister(pSEventBinding);
            EventBus<AddItemEvent>.Deregister(itemEventBinding);
            EventBus<OpenHUDEvent>.Deregister(hudEventBinding);
            inputReader.diceRollActions.onOpenSigilStorage -= HandleSigilStorage;
            inputReader.diceRollActions.onOpenPassiveSigilStorage -= HandlePassiveSigilStorage;
        }

        private void HandleASIC(AddActiveSigilEvent evt)
        {
            activeSigils[evt.index].SetSigilView(evt.sigilSO);
        }

        private void HandlePS(AddPassiveSigilEvent evt)
        {
            passiveSigils[evt.index].SetSigilView(evt.sigilSO);
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

        private void HandleSigilStorage()
        {
            if (!isShowingStorage)
            {
                hudView.ShowStorage();
                isShowingStorage = true;
            }
            else
            {
                hudView.HideStorage();
                isShowingStorage = false;
            }
        }

        private void HandlePassiveSigilStorage()
        {
            if (isShowPassiveSigil)
            {
                hudView.HidePassiveSigilStorage();
                isShowPassiveSigil = false;
            }
            else
            {
                hudView.ShowPassiveSigilStorage();
                isShowPassiveSigil = true;
            }
        }
    }
}