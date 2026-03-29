using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyRule.UI
{
    public class AchievementInfoView : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        [SerializeField] private TextMeshProUGUI achivementName;
        [SerializeField] private Slider achiementProgress;
        [SerializeField] private GameObject select;
        [SerializeField] private GameObject deselect;
        [SerializeField] private GameObject unlockedHightLight;
        [SerializeField] private bool unlocked = false;
        [SerializeField] private GameObject hasReceive;
        [SerializeField] private bool hasReceiveReward = false;
        [SerializeField] private Image rewardIcon;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private Sprite goldSprite;
        [SerializeField] private Sprite crystalSprite;

        [SerializeField] private AchievementConfig achivementConfig;

        private AchievementData achievementData;

        public bool Unlocked => unlocked;

        public void SetAchievement(AchievementData achievementData)
        {
            this.achievementData = achievementData;
            this.achivementConfig = AchievementManager.Instance.GetAchievementById(achievementData.ID);

            if (achievementData.IsUnlocked) unlockedHightLight.SetActive(true);

            achivementName.text = achivementConfig.name;

            if (!hasReceiveReward)
            {
                if (achievementData.IsUnlocked) unlockedHightLight.SetActive(true);
            }
            else
            {
                hasReceive.SetActive(true);
            }

            switch (achivementConfig.rewardType)
            {
                case RewardType.Gold:
                    {
                        rewardIcon.sprite = goldSprite;
                        rewardText.text = achivementConfig.goldReward.ToString();
                        break;
                    }
                case RewardType.Crystal:
                    {
                        rewardIcon.sprite = crystalSprite;
                        rewardText.text = achivementConfig.crystalReward.ToString();
                        break;
                    }
                case RewardType.Sigil:
                    {
                        rewardIcon.sprite = achivementConfig.sigilReward.sigilIcon;
                        rewardText.text = achivementConfig?.sigilReward.sigilName;
                        break;
                    }
                default:
                    {
                        rewardIcon.gameObject.SetActive(false);
                        rewardText.gameObject.SetActive(false);
                        break;
                    }
            }

            achiementProgress.value = achievementData.GetCurrentProgress();
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (hasReceiveReward) return;

            select.SetActive(true);
            deselect.SetActive(false);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (hasReceiveReward) return;

            select.SetActive(false);
            deselect.SetActive(true);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (unlocked)
            {
                hasReceiveReward = true;
                hasReceive.SetActive(true);
                achievementData.ReceiveRewards();   
                AchievementManager.Instance.GiveReward(achivementConfig);
            }
            
            return;
        }
    }
}