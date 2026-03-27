using Cysharp.Threading.Tasks;
using DG.Tweening;
using MyRule.CommandPattern;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MyRule
{
    public class CardDetailView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float fadeDuration = 0.4f;
        
        [Header("Card")]
        [SerializeField] private Transform cardPoint;
        [SerializeField] private SigilSO sigilSO;
        [SerializeField] private Card card;
        private SigilData sigilData;

        [Header("Info")]
        [SerializeField] private Image sigilIcon;
        [SerializeField] private TextMeshProUGUI sigilName;
        [SerializeField] private TextMeshProUGUI sigilaDes;

        [Header("Base Stats")]
        [SerializeField] private TextMeshProUGUI health;
        [SerializeField] private TextMeshProUGUI mana;
        [SerializeField] private TextMeshProUGUI manaCost;
        [SerializeField] private TextMeshProUGUI critChance;
        [SerializeField] private TextMeshProUGUI speed;

        [Header("Damage")]
        [SerializeField] private TextMeshProUGUI physDmg;
        [SerializeField] private TextMeshProUGUI magDmg;
        [SerializeField] private TextMeshProUGUI fireDmg;
        [SerializeField] private TextMeshProUGUI lightningDmg;
        [SerializeField] private TextMeshProUGUI waterDmg;
        [SerializeField] private TextMeshProUGUI frostDmg;
        [SerializeField] private TextMeshProUGUI poisonDmg;
        [SerializeField] private TextMeshProUGUI holyDmg;
        [SerializeField] private TextMeshProUGUI darkDmg;

        [Header("Defense")]
        [SerializeField] private TextMeshProUGUI physDef;
        [SerializeField] private TextMeshProUGUI magDef;
        [SerializeField] private TextMeshProUGUI fireDef;
        [SerializeField] private TextMeshProUGUI lightningDef;
        [SerializeField] private TextMeshProUGUI waterDef;
        [SerializeField] private TextMeshProUGUI frostDef;
        [SerializeField] private TextMeshProUGUI poisonDef;
        [SerializeField] private TextMeshProUGUI holyDef;
        [SerializeField] private TextMeshProUGUI darkDef;

        private bool isShowing = false;

        private EventBinding<ShowCardDetailEvent> showCardDetailEventBinding;

        private void OnEnable()
        {
            showCardDetailEventBinding = new EventBinding<ShowCardDetailEvent>(HandleShowCardDetail);
            EventBus<ShowCardDetailEvent>.Register(showCardDetailEventBinding);
        }

        private void OnDisable()
        {
            EventBus<ShowCardDetailEvent>.Deregister(showCardDetailEventBinding);
        }

        private void HandleShowCardDetail(ShowCardDetailEvent detail)
        {
            sigilSO = detail.sigilSO;
            sigilData = detail.sigilData;
            SetCardDetail();
            SetCardView();

            ICommand command = new CardDetailCommand(this);
            CommandInvoker.ExecuteCommand(command);
        }

        private void SetCardDetail()
        {
            sigilIcon.sprite = sigilSO.sigilIcon;
            sigilName.text = sigilSO.sigilName;
            sigilaDes.text = sigilSO.sigilDesTD;

            health.text = sigilSO.health.ToString();
            manaCost.text = sigilSO.manaCost.ToString();
            critChance.text = sigilSO.critChance.ToString();
            speed.text = sigilSO.speed.ToString();

            physDmg.text = sigilSO.phys.ToString();
            magDmg.text = sigilSO.mag.ToString();
            fireDmg.text = sigilSO.fire.ToString();
            lightningDmg.text = sigilSO.lightning.ToString();
            waterDmg.text = sigilSO.water.ToString();
            poisonDmg.text = sigilSO.poison.ToString();
            holyDmg.text = sigilSO.holy.ToString();
            darkDmg.text = sigilSO.dark.ToString();

            physDef.text = sigilSO.phyDef.ToString();
            magDef.text = sigilSO.magicDef.ToString();
            fireDef.text = sigilSO.fireDef.ToString();
            lightningDef.text = sigilSO.lightningDef.ToString();
            waterDef.text = sigilSO.waterDef.ToString();
            poisonDef.text = sigilSO.poisonDef.ToString();
            holyDef.text = sigilSO.holyDef.ToString();
            darkDef.text = sigilSO.darkDef.ToString();
        }

        private void SetCardView()
        {
            card = CardPoolManager.Instance.Spawn(sigilSO.id);
            card.transform.SetParent(cardPoint);
            card.transform.localPosition = Vector3.zero;
            card.SetSigil(sigilData, sigilSO, CardGameplayType.Detail);
        }

        public void ShowDetail()
        {
            if (isShowing) return;
            
            EventBus<CardDetailLockEvent>.Raise(new CardDetailLockEvent(true));
            CardTracker.Instance.UnlockInteract(false);

            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, fadeDuration);

            isShowing = true;
        }

        public async void HideDetail()
        {
            if (!isShowing) return;

            EventBus<CardDetailLockEvent>.Raise(new CardDetailLockEvent(false));
            CardTracker.Instance.UnlockInteract(true);

            card.ReleasePool();
            await UniTask.Delay(200);
            canvasGroup.alpha = 1f;
            canvasGroup.DOFade(0f, fadeDuration);

            isShowing = false;
        }
    }
}