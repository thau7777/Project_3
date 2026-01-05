using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule.UI
{
    public class HUDController : MonoBehaviour
    {
        public InputReader inputReader;
        public RuneSO runeSO;

        private bool hasTabPressed = false;

        private VisualElement root;
        //private VisualElement sigilStorage;
        //private VisualElement buttonConstraint;
        //private VisualElement fade;
        private VisualElement defaultBtn;
        private VisualElement sigilBtn;
        private Button diceRollBtn;
        private Button statsBtn;
        private Button sigilRollBtn;
        private Button skipBtn;

        private EventBinding<ShowSigilCardEvent> showSigilCardEventBinding;
        //private EventBinding<SigilBoardExitEvent> sigilBoardExitEventBinding;

        private void OnEnable()
        {
            showSigilCardEventBinding = new EventBinding<ShowSigilCardEvent>(OnSigilBoardEnter);
            EventBus<ShowSigilCardEvent>.Register(showSigilCardEventBinding);

            //sigilBoardExitEventBinding = new EventBinding<SigilBoardExitEvent>(evt => ShowHUD());
            //EventBus<SigilBoardExitEvent>.Register(sigilBoardExitEventBinding);

            //inputReader.diceRollActions.onTab += ShowAllSigil;
            inputReader.diceRollActions.onRoll += OnRoll;
        }

        private void OnDisable()
        {
            //EventBus<SigilBoardEnterEvent>.Deregister(sigilBoardEventBinding);

            //EventBus<SigilBoardExitEvent>.Deregister(sigilBoardExitEventBinding);

            //inputReader.diceRollActions.onTab -= ShowAllSigil;
            inputReader.diceRollActions.onRoll -= OnRoll;
        }

        private void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            root = uiDocument.rootVisualElement;

            //sigilStorage = root.Q<VisualElement>("SigilStorage");
            //buttonConstraint = root.Q<VisualElement>("ButtonConstraint");
            //fade = root.Q<VisualElement>("Fade");
            defaultBtn = root.Q<VisualElement>("DefaultBtn");
            sigilBtn = root.Q<VisualElement>("SigilBtn");
            diceRollBtn = root.Q<Button>("DiceRollBtn");
            statsBtn = root.Q<Button>("StatsBtn");
            sigilRollBtn = root.Q<Button>("SigilRollBtn");
            skipBtn = root.Q<Button>("SkipBtn");
            //statsButton = root.Q<Button>("StatsButton");

            inputReader.SwitchActionMap(ActionMap.DiceRoll);
        }

        private void Start()
        {
            diceRollBtn.clicked += OnRoll;
            sigilRollBtn.clicked += OnSigilRoll;
            //statsButton.clicked += () =>
            //{
            //    EventBus<PlayerStatsShowEvent>.Raise(new PlayerStatsShowEvent());
            //};
        }

        private void OnRoll()
        {
            EventBus<DiceRollEvent>.Raise(new DiceRollEvent());
        }

        private void OnSigilRoll()
        {
            if (runeSO.runeCount >= 2)
            {
                EventBus<RollSigilCardEvent>.Raise(new RollSigilCardEvent());
                runeSO.runeCount -= 2;
            }

            return;
        }

        private void OnSigilBoardEnter(ShowSigilCardEvent evt)
        {
            if (evt.showing)
            {
                defaultBtn.AddToClassList("defaultBtn_style_hidden");
                sigilBtn.RemoveFromClassList("sigilBtn_style_hidden");
            }
            else
            {
                defaultBtn.RemoveFromClassList("defaultBtn_style_hidden");
                sigilBtn.AddToClassList("sigilBtn_style_hidden");
            }
        }

        //public void ShowHUD()
        //{
        //    sigilStorage.RemoveFromClassList("sigilStorage_hind");
        //    buttonConstraint.RemoveFromClassList("buttonConstraint_hind");
        //}

        //private void ShowAllSigil()
        //{
        //    if (!hasTabPressed)
        //    {
        //        sigilStorage.AddToClassList("sigilStorage_showall");
        //        buttonConstraint.AddToClassList("buttonConstraint_hind");
        //        fade.style.display = DisplayStyle.Flex;
        //        hasTabPressed = true;
        //    }
        //    else
        //    {
        //        sigilStorage.RemoveFromClassList("sigilStorage_showall");
        //        buttonConstraint.RemoveFromClassList("buttonConstraint_hind");
        //        fade.style.display = DisplayStyle.None;
        //        hasTabPressed = false;
        //    }
        //}
    }
}