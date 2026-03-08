using TMPro;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using MyRule.CommandPattern;

namespace MyRule
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private SigilSO sigilSO;

        [SerializeField] private float hoverScale;
        [SerializeField] private SpriteRenderer sigilImg;
        [SerializeField] private TextMeshPro sigilNameTxt;
        [SerializeField] private TextMeshPro sigilDescTxt;
        [SerializeField] private TextMeshPro runeTxt;
        [SerializeField] private GameObject priceObj;

        [SerializeField] private float showCardDuration = 0.4f;

        private bool isShowing = false;

        public bool IsShowing
        {
            get { return isShowing; } 
            set 
            {
                isShowing = value;
                if (isShowing)
                {
                    ShowCard();
                }
                else
                {
                    HideCard();
                }
            }
        }

        public SigilSO SigilSO => sigilSO;

        private void Start()
        {
        }

        public void SetSigil(SigilSO normalSigilSO)
        {
            sigilSO = normalSigilSO;
            if (sigilImg != null) sigilImg.sprite = normalSigilSO.sigilIcon;
            sigilNameTxt.text = normalSigilSO.sigilName;
            sigilDescTxt.text = normalSigilSO.sigilDesTD;
            if (normalSigilSO.sigilDesTB != null)
            {
                sigilDescTxt.text += '\n' + normalSigilSO.sigilDesTB;
            }
            runeTxt.text = normalSigilSO.price.ToString();
        }

        private void OnMouseEnter()
        {
            if (!isShowing) return;

            transform.localScale *= hoverScale;

            EventBus<HoverSigilCardEvent>.Raise(new HoverSigilCardEvent(this));
        }

        private void OnMouseExit()
        {
            if (!isShowing) return;

            transform.localScale /= hoverScale;

            EventBus<HoverSigilCardEvent>.Raise(new HoverSigilCardEvent(null));
        }

        public void OnClick()
        {
            Debug.Log("Click " + sigilNameTxt.text);
            EventBus<SigilChosenEvent>.Raise(new SigilChosenEvent(sigilSO));
        }

        protected void ShowCard()
        {
            transform.DORotate(new Vector3(0, 0, 0), showCardDuration);
        }

        protected void HideCard()
        {
            transform.DORotate(new Vector3(0, 180, 0), showCardDuration);
        }

        public async void ShowPrice(bool value)
        {
            await UniTask.Delay((int)showCardDuration * 1000);
            priceObj.SetActive(value);
        }
    }
}