using MyRule.Event;
using System.Threading;
using TMPro;
using Cysharp.Threading.Tasks;
using MyRule.UI;

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
        private EventBinding<ShowLobbyEvent> _showEventBinding;

        private CancellationTokenSource cts;

        private LobbyView lobbyView;

        public LobbyPresenter(TextMeshProUGUI goldTxt, TextMeshProUGUI crystalTxt, float transitionDuration, LobbyView lobbyView)
        {
            this.goldTxt = goldTxt;
            this.crystalTxt = crystalTxt;

            this.transitionDuration = transitionDuration;

            this.lobbyView = lobbyView;

            cts = new CancellationTokenSource();

            _goldEventBinding = new EventBinding<UpdateLobbyGoldUIEvent>(HandleGold);
            EventBus<UpdateLobbyGoldUIEvent>.Register(_goldEventBinding);

            _crystalEventBinding = new EventBinding<UpdateLobbyCrystalUIEvent>(HandleCrystal);
            EventBus<UpdateLobbyCrystalUIEvent>.Register(_crystalEventBinding);

            _showEventBinding = new EventBinding<ShowLobbyEvent>(HandleShowLobby);
            EventBus<ShowLobbyEvent>.Register(_showEventBinding);
        }

        public void Clearup()
        {
            EventBus<UpdateLobbyGoldUIEvent>.Deregister(_goldEventBinding);
            EventBus<UpdateLobbyCrystalUIEvent>.Deregister(_crystalEventBinding);
            EventBus<ShowLobbyEvent>.Deregister(_showEventBinding);
        }

        private void HandleGold(UpdateLobbyGoldUIEvent evt)
        {
            Transition.TransitionValue(
                    setter: value => goldTxt.text = value.ToString(),
                    from: currentGold,
                    to: evt.value,
                    duration: transitionDuration,
                    cts.Token).Forget();

            currentGold = evt.value;
        }

        private void HandleCrystal(UpdateLobbyCrystalUIEvent evt)
        {
            Transition.TransitionValue(
                    setter: value => crystalTxt.text = value.ToString(),
                    from: currentCrystal,
                    to: evt.value,
                    duration: transitionDuration,
                    cts.Token).Forget();

            currentCrystal = evt.value;
        }

        private void HandleShowLobby(ShowLobbyEvent evt)
        {
            if (evt.show)
            {
                lobbyView.Show();
            }
            else
            {
                lobbyView.Hide();
            }
        }
    }
}