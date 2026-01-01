using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule.UI
{
    public class HUDController : MonoBehaviour
    {
        public InputReader inputReader;
        private bool hasTabPressed = false;

        private VisualElement root;
        private VisualElement sigilStorage;
        private VisualElement buttonConstraint;
        private VisualElement fade;
        private Button rollButton;
        private Button statsButton;

        private EventBinding<SigilBoardEnterEvent> sigilBoardEventBinding;
        private EventBinding<SigilBoardExitEvent> sigilBoardExitEventBinding;

        private void OnEnable()
        {
            sigilBoardEventBinding = new EventBinding<SigilBoardEnterEvent>(OnSigilBoardEnter);
            EventBus<SigilBoardEnterEvent>.Register(sigilBoardEventBinding);

            sigilBoardExitEventBinding = new EventBinding<SigilBoardExitEvent>(evt => ShowHUD());
            EventBus<SigilBoardExitEvent>.Register(sigilBoardExitEventBinding);

            inputReader.diceRollActions.onTab += ShowAllSigil;
        }

        private void OnDisable()
        {
            EventBus<SigilBoardEnterEvent>.Deregister(sigilBoardEventBinding);

            EventBus<SigilBoardExitEvent>.Deregister(sigilBoardExitEventBinding);

            inputReader.diceRollActions.onTab -= ShowAllSigil;
        }

        private void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            root = uiDocument.rootVisualElement;

            sigilStorage = root.Q<VisualElement>("SigilStorage");
            buttonConstraint = root.Q<VisualElement>("ButtonConstraint");
            fade = root.Q<VisualElement>("Fade");
            rollButton = root.Q<Button>("RollButton");
            statsButton = root.Q<Button>("StatsButton");

            inputReader.SwitchActionMap(ActionMap.PlayerFPS);
        }

        private void Start()
        {
            rollButton.clicked += () =>
            {
                EventBus<DiceRollEvent>.Raise(new DiceRollEvent());
            };
            statsButton.clicked += () =>
            {
                EventBus<PlayerStatsShowEvent>.Raise(new PlayerStatsShowEvent());
            };
        }

        private void OnSigilBoardEnter(SigilBoardEnterEvent evt)
        {
            sigilStorage.AddToClassList("sigilStorage_hind");
            buttonConstraint.AddToClassList("buttonConstraint_hind");
        }

        public void ShowHUD()
        {
            sigilStorage.RemoveFromClassList("sigilStorage_hind");
            buttonConstraint.RemoveFromClassList("buttonConstraint_hind");
        }

        private void ShowAllSigil()
        {
            if (!hasTabPressed)
            {
                sigilStorage.AddToClassList("sigilStorage_showall");
                buttonConstraint.AddToClassList("buttonConstraint_hind");
                fade.style.display = DisplayStyle.Flex;
                hasTabPressed = true;
            }
            else
            {
                sigilStorage.RemoveFromClassList("sigilStorage_showall");
                buttonConstraint.RemoveFromClassList("buttonConstraint_hind");
                fade.style.display = DisplayStyle.None;
                hasTabPressed = false;
            }
        }
    }
}