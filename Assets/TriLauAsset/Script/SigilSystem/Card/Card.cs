using TMPro;
using UnityEngine;
using DG.Tweening;

namespace MyRule
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private SigilSO sigilSO;

        [SerializeField] private float hoverScale;
        [SerializeField] private SpriteRenderer sigilImg;
        [SerializeField] private TextMeshProUGUI sigilNameTxt;
        [SerializeField] private TextMeshProUGUI sigilDescTxt;

        [SerializeField] private float duration;

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

        protected void ShowCard()
        {
            transform.DORotate(new Vector3(0, 0, 0), duration);
        }

        protected void HideCard()
        {
            transform.DORotate(new Vector3(0, 180, 0), duration);
        }
    }
}