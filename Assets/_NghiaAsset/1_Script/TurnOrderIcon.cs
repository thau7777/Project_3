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
        public TextMeshProUGUI roundTrackerText;

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

            if (roundTrackerText != null)
            {
                roundTrackerText.gameObject.SetActive(false);
            }

            if (avatarImage != null && character.info.Avatar != null)
            {
                avatarImage.sprite = character.info.Avatar;
            }

            if (character.isVirtualTracker)
            {
                avatarButton.interactable = false;

                if (roundTrackerText != null && character is RoundTracker roundTracker)
                {
                    roundTrackerText.text = $"{roundTracker.currentRound}";
                    roundTrackerText.gameObject.SetActive(true);
                }

                if (actionGaugeText != null)
                {
                    actionGaugeText.gameObject.SetActive(true);
                }
            }

            if (actionGaugeText != null)
            {
                float displayValue = Mathf.Min(character.actionGauge, 100f);
                actionGaugeText.text = Mathf.RoundToInt(displayValue).ToString();
            }

            if (cameraViewManager != null)
            {
                cameraViewManager.SetCameraView(characterOwner);
            }

            avatarButton.onClick.RemoveAllListeners();
            avatarButton.onClick.AddListener(ShowPanel);

            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            if (characterOwner != null && actionGaugeText != null)
            {
                float displayValue = Mathf.Min(characterOwner.actionGauge, 100f);
                actionGaugeText.text = Mathf.RoundToInt(displayValue).ToString();
            }

            if (characterOwner is RoundTracker roundTracker && roundTrackerText != null && characterOwner.isVirtualTracker)
            {
                roundTrackerText.text = $"{roundTracker.currentRound}";
                roundTrackerText.gameObject.SetActive(true);
            }
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