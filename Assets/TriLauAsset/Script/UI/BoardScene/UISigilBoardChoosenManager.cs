using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule.UI
{
    public class UISigilBoardChoosenManager : MonoBehaviour
    {
        public GroupSigil groupSigil;
        public RuneSO runeSO;

        private VisualElement sigilBoard;

        private Label sigil1Icon;
        private Label sigil1Name;
        private Label sigil1Des;

        private Label sigil2Icon;
        private Label sigil2Name;
        private Label sigil2Des;

        private Label sigil3Icon;
        private Label sigil3Name;
        private Label sigil3Des;

        private Button sigil1Button;
        private Button sigil2Button;
        private Button sigil3Button;

        private Button rollButton;
        private Button skipButton;

        private NormalSigilSO sigil1;
        private NormalSigilSO sigil2;
        private NormalSigilSO sigil3;

        private EventBinding<SigilBoardEnterEvent> sigilBoardEventBinding;

        private void OnEnable()
        {
            sigilBoardEventBinding = new EventBinding<SigilBoardEnterEvent>(OnSigilBoardEnter);
            EventBus<SigilBoardEnterEvent>.Register(sigilBoardEventBinding);
        }

        private void OnDisable()
        {
            EventBus<SigilBoardEnterEvent>.Deregister(sigilBoardEventBinding);
        }

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            sigilBoard = root.Q<VisualElement>("SigilBoard");

            sigil1Icon = root.Q<Label>("Sigil1Icon");
            sigil1Name = root.Q<Label>("Sigil1Name");
            sigil1Des = root.Q<Label>("Sigil1Des");

            sigil2Icon = root.Q<Label>("Sigil2Icon");
            sigil2Name = root.Q<Label>("Sigil2Name");
            sigil2Des = root.Q<Label>("Sigil2Des");

            sigil3Icon = root.Q<Label>("Sigil3Icon");
            sigil3Name = root.Q<Label>("Sigil3Name");
            sigil3Des = root.Q<Label>("Sigil3Des");

            sigil1Button = root.Q<Button>("SigilButton1");
            sigil2Button = root.Q<Button>("SigilButton2");
            sigil3Button = root.Q<Button>("SigilButton3");

            rollButton = root.Q<Button>("RollSigilButton");
            skipButton = root.Q<Button>("SkipButton");
        }

        private void Start()
        {
            HideBoard();

            rollButton.clicked += OnRollButtonClicked;
            skipButton.clicked += Skip();

            sigil1Button.clicked += () =>
            {
                HideBoard();
                EventBus<SigilChosenEvent>.Raise(new SigilChosenEvent(sigil1));
                EventBus<SigilBoardExitEvent>.Raise(new SigilBoardExitEvent());
            };

            sigil2Button.clicked += () =>
            {
                HideBoard();
                EventBus<SigilChosenEvent>.Raise(new SigilChosenEvent(sigil2));
                EventBus<SigilBoardExitEvent>.Raise(new SigilBoardExitEvent());
            };

            sigil3Button.clicked += () =>
            {
                HideBoard();
                EventBus<SigilChosenEvent>.Raise(new SigilChosenEvent(sigil3));
                EventBus<SigilBoardExitEvent>.Raise(new SigilBoardExitEvent());
            };
        }

        private void OnSigilBoardEnter(SigilBoardEnterEvent evt)
        {
            ShowBoard();

            RandomSigil();
        }

        private void OnRollButtonClicked()
        {
            if (runeSO.runeCount < 2)
            {
                return;
            }

            RandomSigil();

            runeSO.runeCount -= 2;
        }

        private void RandomSigil()
        {
            sigil1 = GetWeightedRandom();
            sigil1Icon.style.backgroundImage = sigil1.sigilIcon;
            sigil1Name.text = sigil1.sigilName;
            sigil1Des.text = sigil1.sigilDesTD;
            if (sigil1.sigilDesTB != null)
            {
                sigil1Des.text += '\n' + sigil1.sigilDesTB;
            }

            sigil2 = GetWeightedRandom();
            sigil2Icon.style.backgroundImage = sigil2.sigilIcon;
            sigil2Name.text = sigil2.sigilName;
            sigil2Des.text = sigil2.sigilDesTD;
            if (sigil2.sigilDesTD != null)
            {
                sigil2Des.text += '\n' + sigil2.sigilDesTB;
            }

            sigil3 = GetWeightedRandom();
            sigil3Icon.style.backgroundImage = sigil3.sigilIcon;
            sigil3Name.text = sigil3.sigilName;
            sigil3Des.text = sigil3.sigilDesTD;
            if (sigil3.sigilDesTD != null)
            {
                sigil3Des.text += '\n' + sigil3.sigilDesTB;
            }
        }

        private void ShowBoard()
        {
            sigilBoard.RemoveFromClassList("sigilBoard_style_hidden");
        }   
        
        private void HideBoard()
        {
            sigilBoard.AddToClassList("sigilBoard_style_hidden");
        }

        private NormalSigilSO GetWeightedRandom()
        {
            int totalWeight = 0;
            foreach (var s in groupSigil.normalSigil)
                totalWeight += s.rarity;

            int random = Random.Range(0, totalWeight);
            int current = 0;

            foreach (var s in groupSigil.normalSigil)
            {
                current += s.rarity;
                if (random < current)
                    return s;
            }

            return groupSigil.normalSigil[0];
        }

        private System.Action Skip()
        {
            return () =>
            {
                HideBoard();
                EventBus<SigilBoardExitEvent>.Raise(new SigilBoardExitEvent());
            };
        }
    }
}