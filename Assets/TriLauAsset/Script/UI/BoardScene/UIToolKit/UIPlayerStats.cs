using UnityEngine;
using UnityEngine.UIElements;

namespace MyRule.UI
{
    public class UIPlayerStats : MonoBehaviour
    {
        public RuneSO runeSO;
        public CharacterStatsSO characterStatsSO;

        private VisualElement playerStats;
        private Button exitStatsButton;
        private Label rune;
        private Button virgorPlus;
        private Button mindPlus;
        private Button endurancePlus;
        private Button strPlus;
        private Button dexPlus;
        private Button intelPlus;
        private Button faiPlus;
        private Button arcPlus;

        private int baseHp;
        private int baseFp;
        private int baseStamina;
        private int basePhysDmg;
        private int baseMagDmg;
        private float baseCritChance;
        private float baseCritMult;

        private EventBinding<PlayerStatsShowEvent> playerStatsShowEventBinding;

        private void OnEnable()
        {
            playerStatsShowEventBinding = new EventBinding<PlayerStatsShowEvent>(evt => Show());
            EventBus<PlayerStatsShowEvent>.Register(playerStatsShowEventBinding);
        }

        private void OnDisable()
        {
            EventBus<PlayerStatsShowEvent>.Deregister(playerStatsShowEventBinding);
        }

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            playerStats = root.Q<VisualElement>("PlayerStats");
            exitStatsButton = root.Q<Button>("ExitStatsButton");
            rune = root.Q<Label>("Rune");
            virgorPlus = root.Q<Button>("VirgorPlus");
            mindPlus = root.Q<Button>("MindPlus");
            endurancePlus = root.Q<Button>("EndurancePlus");
            strPlus = root.Q<Button>("StrPlus");
            dexPlus = root.Q<Button>("DexPlus");
            intelPlus = root.Q<Button>("IntelPlus");
            faiPlus = root.Q<Button>("FaiPlus");
            arcPlus = root.Q<Button>("ArcPlus");
        }

        private void Start()
        {
            Hide();

            SetBase();

            UpdateStats();

            exitStatsButton.clicked += () =>
            {
                Hide();
            };

            virgorPlus.clicked += () =>
            {
                Plus(ref characterStatsSO.virgor, ref runeSO.runeAmount, ref characterStatsSO.rune);
            };

            mindPlus.clicked += () =>
            {
                Plus(ref characterStatsSO.mind, ref runeSO.runeAmount, ref characterStatsSO.rune);
            };

            endurancePlus.clicked += () =>
            {
                Plus(ref characterStatsSO.endurance, ref runeSO.runeAmount, ref characterStatsSO.rune);
            };

            strPlus.clicked += () =>
            {
                Plus(ref characterStatsSO.strength, ref runeSO.runeAmount, ref characterStatsSO.rune);
            };

            dexPlus.clicked += () =>
            {
                Plus(ref characterStatsSO.dexterity, ref runeSO.runeAmount, ref characterStatsSO.rune);
            };

            intelPlus.clicked += () =>
            {
                Plus(ref characterStatsSO.intelligence, ref runeSO.runeAmount, ref characterStatsSO.rune);
            };

            faiPlus.clicked += () =>
            {
                Plus(ref characterStatsSO.faith, ref runeSO.runeAmount, ref characterStatsSO.rune);
            };

            arcPlus.clicked += () =>
            {
                Plus(ref characterStatsSO.arcane, ref runeSO.runeAmount, ref characterStatsSO.rune);
            };
        }

        private void Update()
        {
            if (runeSO.runeAmount >= characterStatsSO.rune)
            {
                ShowPlusButton();
            }
            else
            {
                HidePlusButton();
            }
        }

        private void Show()
        {
            playerStats.style.display = DisplayStyle.Flex;
            rune.style.display = DisplayStyle.None;
        }

        private void Hide()
        {
            playerStats.style.display = DisplayStyle.None;
            rune.style.display = DisplayStyle.Flex;
        }

        private void ShowPlusButton()
        {
            virgorPlus.style.visibility = Visibility.Visible;
            mindPlus.style.visibility = Visibility.Visible;
            endurancePlus.style.visibility = Visibility.Visible;
            strPlus.style.visibility = Visibility.Visible;
            dexPlus.style.visibility = Visibility.Visible;
            intelPlus.style.visibility = Visibility.Visible;
            faiPlus.style.visibility = Visibility.Visible;
            arcPlus.style.visibility = Visibility.Visible;
        }

        private void HidePlusButton()
        {
            virgorPlus.style.visibility = Visibility.Hidden;
            mindPlus.style.visibility = Visibility.Hidden;
            endurancePlus.style.visibility = Visibility.Hidden;
            strPlus.style.visibility = Visibility.Hidden;
            dexPlus.style.visibility = Visibility.Hidden;
            intelPlus.style.visibility = Visibility.Hidden;
            faiPlus.style.visibility = Visibility.Hidden;
            arcPlus.style.visibility = Visibility.Hidden;
        }

        private void Plus(ref int stats, ref int runeCount, ref int runeNeed)
        {
            stats += 1;

            runeCount -= characterStatsSO.rune;

            runeNeed += characterStatsSO.rune;

            UpdateStats();
        }

        private void SetBase()
        {
            baseHp = characterStatsSO.hp;
            baseFp = characterStatsSO.fp;
            baseStamina = characterStatsSO.stamina;
            basePhysDmg = characterStatsSO.attackDmg;
            baseMagDmg = characterStatsSO.magicDmg;
            baseCritChance = characterStatsSO.critChance;
            baseCritMult = characterStatsSO.critMult;
        }

        private void UpdateStats()
        {
            characterStatsSO.hp = (int) (baseHp * GetMultiplier.GetStatMultiplier(characterStatsSO.virgor));
            characterStatsSO.fp = (int) (baseFp * GetMultiplier.GetStatMultiplier(characterStatsSO.mind));
            characterStatsSO.stamina = (int) (baseStamina * GetMultiplier.GetStatMultiplier(characterStatsSO.endurance));
            characterStatsSO.attackDmg = (int) (basePhysDmg * GetMultiplier.GetStatMultiplier(characterStatsSO.strength));
            characterStatsSO.magicDmg = (int) (baseMagDmg * GetMultiplier.GetStatMultiplier(characterStatsSO.intelligence));
        }
    }
}