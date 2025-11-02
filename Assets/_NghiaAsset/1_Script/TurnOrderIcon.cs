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

        public CameraViewManager cameraViewManager;

        void Awake()
        {
            cameraViewManager = FindFirstObjectByType<CameraViewManager>();
        }

        public void Setup(Character character, CharacterStatUI statUIRef, BattleManager battleManagerRef)
        {
            characterOwner = character;
            statDisplayPanel = statUIRef;
            battleManager = battleManagerRef;

            if (avatarImage != null && character.info.Avatar != null)
            {
                avatarImage.sprite = character.info.Avatar;
            }

            if (actionGaugeText != null)
            {
                actionGaugeText.text = Mathf.RoundToInt(character.actionGauge).ToString();
            }

            if (cameraViewManager != null)
            {
                cameraViewManager.SetCameraView(characterOwner);
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

            if (cameraViewManager != null)
            {
                cameraViewManager.SetCameraView(characterOwner);
            }
        }
    }
}