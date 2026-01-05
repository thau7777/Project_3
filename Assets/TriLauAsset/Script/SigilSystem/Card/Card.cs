using TMPro;
using UnityEngine;

namespace MyRule
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private Vector3 hoverScale;
        [SerializeField] private SpriteRenderer sigilImg;
        [SerializeField] private TextMeshProUGUI sigilNameTxt;
        [SerializeField] private TextMeshProUGUI sigilDescTxt;

        private bool showing = false;

        public bool Showing
        {
            get { return showing; }
            set 
            { 
                showing = value;
                
                animator.SetBool("Show", showing);
            }
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void SetSigil(NormalSigilSO normalSigilSO)
        {
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
            transform.localScale += hoverScale;

            EventBus<HoverSigilCardEvent>.Raise(new HoverSigilCardEvent(this));
        }

        private void OnMouseExit()
        {
            transform.localScale -= hoverScale;

            EventBus<HoverSigilCardEvent>.Raise(new HoverSigilCardEvent(null));
        }
    }
}