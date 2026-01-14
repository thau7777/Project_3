using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule.UI
{
    public class HUDController : MonoBehaviour
    {
        public InputReader inputReader;
        public RuneSO runeSO;

        //private bool hasTabPressed = false;

        private VisualElement root;
        private VisualElement defaultBtn;
        private VisualElement sigilBtn;
        private Button diceRollBtn;
        private Button statsBtn;
        private Button sigilRollBtn;
        private Button skipBtn;

        private EventBinding<ShowSigilCardEvent> showSigilCardEventBinding;
        
        private void OnEnable()
        {
            showSigilCardEventBinding = new EventBinding<ShowSigilCardEvent>(OnSigilBoardEnter);
            EventBus<ShowSigilCardEvent>.Register(showSigilCardEventBinding);

            inputReader.diceRollActions.onRoll += OnRoll;
        }

        private void OnDisable()
        {
            inputReader.diceRollActions.onRoll -= OnRoll;
        }

        private void Awake()
        {
            var uiDocument = GetComponent<UIDocument>();
            root = uiDocument.rootVisualElement;

            defaultBtn = root.Q<VisualElement>("DefaultBtn");
            sigilBtn = root.Q<VisualElement>("SigilBtn");
            diceRollBtn = root.Q<Button>("DiceRollBtn");
            statsBtn = root.Q<Button>("StatsBtn");
            sigilRollBtn = root.Q<Button>("SigilRollBtn");
            skipBtn = root.Q<Button>("SkipBtn");
            
            inputReader.SwitchActionMap(ActionMap.DiceRoll);
        }

        private void Start()
        {
            diceRollBtn.clicked += OnRoll;
            sigilRollBtn.clicked += OnSigilRoll;
            skipBtn.clicked += OnSkip;
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

        private void OnSkip()
        {
            EventBus<ShowSigilCardEvent>.Raise(new ShowSigilCardEvent(false));
        }

        //public void ShowHUD()
        //{
        //    sigilStorage.RemoveFromClassList("sigilStorage_hind");
        //    buttonConstraint.RemoveFromClassList("buttonConstraint_hind");
        //}
    }
}