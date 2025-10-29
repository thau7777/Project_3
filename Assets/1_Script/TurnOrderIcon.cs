using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Turnbase
{
    public class TurnOrderIcon : MonoBehaviour
    {
        [Header("UI References")]
        public Image avatarImage;
        public Button avatarButton;

        public TextMeshProUGUI actionGaugeText;

        [HideInInspector] public Character characterOwner;

        private CharacterStatUI statDisplayPanel;

        private BattleManager battleManager;




        public void Setup(Character character, CharacterStatUI statUIRef, BattleManager battleManagerRef)
        {
            characterOwner = character;
            statDisplayPanel = statUIRef;
            battleManager = battleManagerRef;

            if (avatarImage != null && character.stats.Avatar != null)
            {
                avatarImage.sprite = character.stats.Avatar;
            }

            if (actionGaugeText != null)
            {
                actionGaugeText.text = Mathf.RoundToInt(character.actionGauge).ToString();
            }

            avatarButton.onClick.RemoveAllListeners();
            avatarButton.onClick.AddListener(ShowPanel);
        }

        private void ShowPanel()
        {
            if (statDisplayPanel != null && characterOwner != null)
            {
                statDisplayPanel.ShowStats(characterOwner);

                if (battleManager != null)
                {
                    battleManager.ShowCombatantButtonsForFaction(characterOwner.isPlayer);
                }

                EventBus<ShowPanelEvent>.Raise(new ShowPanelEvent(panelName: "AvatarCharacterPanel"));
            }
        }
    }
}