using TMPro;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Pool;
using DG.Tweening;

namespace MyRule
{
    public enum CardGameplayType
    {
        Reward,
        StoreItem,
        Detail,
    }

    public class Card : MonoBehaviour
    {
        [SerializeField] private SigilSO sigilSO;
        private SigilData sigilData;

        [SerializeField] private Vector3 originalScale;
        [SerializeField] private Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f);
        [SerializeField] private SpriteRenderer sigilImg;
        [SerializeField] private TextMeshPro sigilNameTxt;
        [SerializeField] private TextMeshPro sigilDescTxt;
        [SerializeField] private MeshRenderer borderMesh;
        [SerializeField] private TextMeshPro runeTxt;
        [SerializeField] private GameObject priceObj;
        [SerializeField] private float showCardDuration = 0.4f;
        [SerializeField] private GameObject sigilReplacedContainer;
        [SerializeField] private SpriteRenderer sigilReplacedIcon;
        [SerializeField] private SigilSO sigilReplaced;
        [SerializeField] private int replaceIndex = -1;
        [SerializeField] private CardRotator cardRotator;
        [SerializeField] private CardClickAnimate clickAnimate;

        public CardRotator CardRotator => cardRotator;
        public CardClickAnimate ClickAnimate => clickAnimate;

        private bool isShowing = false;

        private bool isMouseTray = false;

        private bool locking = false;

        private CardGameplayType cardType;
        public CardGameplayType CardType => cardType; 

        private EventBinding<CheckSigilReplaced> checkReplacedEB;
        private EventBinding<CardDetailLockEvent> lockHoverEB;

        private void OnEnable()
        {
            checkReplacedEB = new EventBinding<CheckSigilReplaced>(CheckSigilStorage);
            EventBus<CheckSigilReplaced>.Register(checkReplacedEB);

            lockHoverEB = new EventBinding<CardDetailLockEvent>(LockHover);
            EventBus<CardDetailLockEvent>.Register(lockHoverEB);
        }

        private void OnDisable()
        {
            EventBus<CheckSigilReplaced>.Deregister(checkReplacedEB);
            EventBus<CardDetailLockEvent>.Deregister(lockHoverEB);
        }

        private void Awake()
        {
            originalScale = Vector3.one;
        }

        private IObjectPool<Card> pool;

        public void SetPool(IObjectPool<Card> objectPool) => this.pool = objectPool;

        public void ReleasePool() => pool.Release(this);

        public SigilSO SigilSO => sigilSO;

        public SigilData SigilData => sigilData;

        public void SetCardGameplayType(CardGameplayType cardGameplayType) => this.cardType = cardGameplayType;

        public async void SetSigil(SigilData sigilData, SigilSO sigilSO, CardGameplayType cardGameplayType)
        {
            await UniTask.NextFrame();

            this.sigilData = sigilData;
            this.sigilSO = sigilSO;
            this.cardType = cardGameplayType;

            if (sigilImg != null) sigilImg.sprite = sigilSO.sigilIcon;
            sigilNameTxt.text = sigilSO.sigilName;
            sigilDescTxt.text = sigilSO.sigilDesTD;
            if (sigilSO.sigilDesTB != null)
            {
                sigilDescTxt.text += '\n' + sigilSO.sigilDesTB;
            }

            switch (sigilSO.rarity)
            {
                case ERarity.Common:
                    borderMesh.material = CardBorderManger.Instance.GetMaterial(0);
                    break;
                case ERarity.Uncommon:
                    borderMesh.material = CardBorderManger.Instance.GetMaterial(1);
                    break;
                case ERarity.Rare:
                    borderMesh.material = CardBorderManger.Instance.GetMaterial(2);
                    break;
                case ERarity.Epic:
                    borderMesh.material = CardBorderManger.Instance.GetMaterial(3);
                    break;
                case ERarity.Legendary:
                    borderMesh.material = CardBorderManger.Instance.GetMaterial(4);
                    break;
                case ERarity.Mythic:
                    borderMesh.material = CardBorderManger.Instance.GetMaterial(5);
                    break;
            }

            CheckSigilStorage();

            ShowPrice();
        }

        private void CheckSigilStorage()
        {
            replaceIndex = -1;
            sigilReplacedContainer.SetActive(false);

            if (sigilSO == null) return;

            if (sigilSO.sigilType == SigilType.Active)
            {
                if (SigilStorageManager.Instance.SigilStorageData.IsActiveSigilFull())
                {
                    sigilReplacedContainer.SetActive(true);
                    SigilStorageSlotData slotData = SigilStorageManager.Instance.GetRandomActiveSigilInStorage();
                    if (slotData != null)
                    {
                        sigilReplaced = SigilCollectionManager.Instance.GetSigilSOById(slotData.Data.Id);
                        sigilReplacedIcon.sprite = sigilReplaced.sigilIcon;
                        replaceIndex = slotData.Index;
                    }
                }
            }
            else if (sigilSO.sigilType == SigilType.Passive)
            {
                if (SigilStorageManager.Instance.SigilStorageData.IsPassiveSigilFull())
                {
                    sigilReplacedContainer.SetActive(true);
                    SigilStorageSlotData slotData = SigilStorageManager.Instance.GetRandomPassiveSigilInStorage();
                    if (slotData != null)
                    {
                        sigilReplaced = SigilCollectionManager.Instance.GetSigilSOById(slotData.Data.Id);
                        sigilReplacedIcon.sprite = sigilReplaced.sigilIcon;
                        replaceIndex = slotData.Index;
                    }
                }
            }
        }   

        #region Hover
        private void OnMouseEnter()
        {
            if (isMouseTray) return;

            if (!isShowing || cardType == CardGameplayType.Detail || locking) return;

            transform.DOScale(hoverScale, 0.1f);

            EventBus<HoverCardEvent>.Raise(new HoverCardEvent(this));

            isMouseTray = true;
        }

        private void OnMouseOver()
        {
            if (isMouseTray) return;

            if (!isShowing || cardType == CardGameplayType.Detail || locking) return;

            transform.DOScale(hoverScale, 0.1f);

            EventBus<HoverCardEvent>.Raise(new HoverCardEvent(this));

            isMouseTray = true;
        }

        private void OnMouseExit()
        {
            if (!isMouseTray) return;
            if (!isShowing || cardType == CardGameplayType.Detail || locking) return;

            transform.DOScale(originalScale, 0.1f);

            EventBus<HoverCardEvent>.Raise(new HoverCardEvent(null));
            isMouseTray = false;
        }
        #endregion

        public async void OnClick()
        {
            if (!isShowing || cardType == CardGameplayType.Detail) return;

            switch (cardType)
            {
                case CardGameplayType.Reward:
                    {
                        await clickAnimate.PlayCircular();
                        await UniTask.Delay(200);
                        EventBus<SigilChosenEvent>.Raise(new SigilChosenEvent(sigilSO, replaceIndex));
                        MatchManager.Instance.MatchData.SigilPool.RemoveSigil(sigilData);
                        break;
                    }
                case CardGameplayType.StoreItem:
                    {
                        int runeAmount = RuneManger.Instance.CurrentRuneAmount;

                        if (runeAmount > sigilSO.price)
                        {
                            clickAnimate.FlipDown().Forget();
                            Debug.Log("Click " + sigilNameTxt.text);
                            EventBus<SigilChosenEvent>.Raise(new SigilChosenEvent(sigilSO, replaceIndex));
                            EventBus<SpendRuneEvent>.Raise(new SpendRuneEvent(sigilSO.price));
                            priceObj.SetActive(false);
                            isShowing = false;
                            transform.localScale = originalScale;
                            MatchManager.Instance.MatchData.SigilPool.RemoveSigil(sigilData);
                        }
                        else
                        {
                            Debug.Log("Ko du tien");
                            clickAnimate.PlayShake().Forget();
                        }
                        break;
                    }
            }
        }

        private async void ShowPrice()
        {
            priceObj.SetActive(false);

            if (cardType != CardGameplayType.StoreItem) return;

            await UniTask.Delay((int)showCardDuration * 1000);
            priceObj.SetActive(true);
            runeTxt.text = sigilSO.price.ToString();
        }

        public async UniTask OnSpawn()
        {
            transform.localScale = Vector3.one;

            await clickAnimate.FlipUp();
            isShowing = true;
        }

        public async UniTask OnDespawn()
        {
            await clickAnimate.FlipDown();
            isShowing = false;
        }

        private void LockHover(CardDetailLockEvent evt)
        {
            transform.DOScale(originalScale, 0.1f);
            locking = evt.locking;
        }
    }
}