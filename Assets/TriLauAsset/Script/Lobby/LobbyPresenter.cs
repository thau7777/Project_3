using MyRule.Event;
using System.Threading;
using TMPro;
using Cysharp.Threading.Tasks;

namespace MyRule
{
    public class LobbyPresenter
    {
        private int currentGold = 0;
        private int currentCrystal = 0;

        private float transitionDuration;

        private TextMeshProUGUI goldTxt;
        private TextMeshProUGUI crystalTxt;

        private EventBinding<UpdateLobbyGoldUIEvent> _goldEventBinding;
        private EventBinding<UpdateLobbyCrystalUIEvent> _crystalEventBinding;

        private CancellationTokenSource cts;

        public LobbyPresenter(TextMeshProUGUI goldTxt, TextMeshProUGUI crystalTxt, float transitionDuration)
        {
            this.goldTxt = goldTxt;
            this.crystalTxt = crystalTxt;

            this.transitionDuration = transitionDuration;

            cts = new CancellationTokenSource();

            _goldEventBinding = new EventBinding<UpdateLobbyGoldUIEvent>(HandleGold);
            EventBus<UpdateLobbyGoldUIEvent>.Register(_goldEventBinding);

            _crystalEventBinding = new EventBinding<UpdateLobbyCrystalUIEvent>(HandleCrystal);
            EventBus<UpdateLobbyCrystalUIEvent>.Register(_crystalEventBinding);
        }

        public void Clearup()
        {
            EventBus<UpdateLobbyGoldUIEvent>.Deregister(_goldEventBinding);
            EventBus<UpdateLobbyCrystalUIEvent>.Deregister(_crystalEventBinding);
        }

        private void HandleGold()
        {
            cts.Cancel();
            cts.Dispose();

            Transition.TransitionValue(
                    setter: value => goldTxt.text = value.ToString(),
                    from: currentGold,
                    to: 0f,
                    duration: transitionDuration,
                    cts.Token).Forget();
        }

        private void HandleCrystal()
        {
            cts.Cancel();
            cts.Dispose();

            Transition.TransitionValue(
                    setter: value => crystalTxt.text = value.ToString(),
                    from: currentCrystal,
                    to: 0f,
                    duration: transitionDuration,
                    cts.Token).Forget();
        }
    }
}