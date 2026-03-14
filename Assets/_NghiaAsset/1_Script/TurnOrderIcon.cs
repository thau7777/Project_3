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

            if (avatarImage != null && character.info != null && character.info.Avatar != null)
            {
                avatarImage.sprite = character.info.Avatar;
            }

            if (character.isVirtualTracker)
            {
                avatarButton.interactable = false;

                if (roundTrackerText != null && character is RoundTracker roundTracker)
                {
                    roundTrackerText.text = "Round";
                    roundTrackerText.gameObject.SetActive(true);
                }
            }

            avatarButton.onClick.RemoveAllListeners();
            avatarButton.onClick.AddListener(ShowPanel);

            UpdateDisplay();
        }

        public void UpdateDisplay()
        {
            if (characterOwner == null || actionGaugeText == null) return;

            float currentSpeed = characterOwner.stats.speed;

            if (characterOwner.actionGauge >= 9999.9f)
            {
                actionGaugeText.text = "0";
                actionGaugeText.color = Color.yellow;
            }
            else
            {
                float avValue = (10000f - characterOwner.actionGauge) / Mathf.Max(1f, currentSpeed);
                int displayAV = Mathf.CeilToInt(avValue);

                actionGaugeText.text = displayAV.ToString();
                actionGaugeText.color = Color.white;
            }

            if (characterOwner is RoundTracker roundTracker && roundTrackerText != null)
            {
                roundTrackerText.text = "Round";
                roundTrackerText.gameObject.SetActive(true);

                if (characterOwner.isVirtualTracker)
                {
                    actionGaugeText.color = new Color(1f, 0.6f, 0f); 
                }
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