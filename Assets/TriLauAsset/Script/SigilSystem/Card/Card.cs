using TMPro;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace MyRule
{
    public class Card : MonoBehaviour
    {
        [SerializeField] private SigilSO sigilSO;
        private SigilData sigilData;

        [SerializeField] private MeshRenderer borderMesh;
        [SerializeField] private Material[] borderMaterial;

        [SerializeField] private Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f);
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

        public void SetSigil(SigilData sigilData, SigilSO sigilSO)
        {
            this.sigilData = sigilData;
            this.sigilSO = sigilSO;
            if (sigilImg != null) sigilImg.sprite = sigilSO.sigilIcon;
            sigilNameTxt.text = sigilSO.sigilName;
            sigilDescTxt.text = sigilSO.sigilDesTD;
            if (sigilSO.sigilDesTB != null)
            {
                sigilDescTxt.text += '\n' + sigilSO.sigilDesTB;
            }
            runeTxt.text = sigilSO.price.ToString();

            switch (sigilSO.rarity)
            {
                case ERarity.Common:
                    borderMesh.material = borderMaterial[0];
                    break;
                case ERarity.Uncommon:
                    borderMesh.material = borderMaterial[1];
                    break;
                case ERarity.Rare:
                    borderMesh.material = borderMaterial[2];
                    break;
                case ERarity.Epic:
                    borderMesh.material = borderMaterial[3];
                    break;
                case ERarity.Legendary:
                    borderMesh.material = borderMaterial[4];
                    break;
                case ERarity.Mythic:
                    borderMesh.material = borderMaterial[5];
                    break;
            }
        }

        private void OnMouseEnter()
        {
            if (!isShowing) return;

            transform.localScale = hoverScale;

            EventBus<HoverSigilCardEvent>.Raise(new HoverSigilCardEvent(this));
        }

        private void OnMouseExit()
        {
            if (!isShowing) return;

            transform.localScale = new Vector3(1, 1, 1);

            EventBus<HoverSigilCardEvent>.Raise(new HoverSigilCardEvent(null));
        }

        public void OnClick(bool isReward)
        {
            if (!isShowing) return;

            if (!isReward)
            {
                int runeAmount = RuneManger.Instance.CurrentRuneAmount;

                if (runeAmount > sigilSO.price)
                {
                    Debug.Log("Click " + sigilNameTxt.text);
                    EventBus<SigilChosenEvent>.Raise(new SigilChosenEvent(sigilSO));
                    EventBus<ReceiveRuneEvent>.Raise(new ReceiveRuneEvent(-sigilSO.price));
                    HideCard();
                    isShowing = false;
                    MatchManager.Instance.RemoveSigilInMatch(sigilData);
                }
                else
                {
                    Debug.Log("Ko du tien");
                }
            }
            else if (isReward)
            {
                EventBus<SigilChosenEvent>.Raise(new SigilChosenEvent(sigilSO));
                MatchManager.Instance.RemoveSigilInMatch(sigilData);
            }
        }

        protected void ShowCard()
        {
            transform.DORotate(new Vector3(0, 0, 0), showCardDuration);
        }

        protected void HideCard()
        {
            transform.DORotate(new Vector3(0, 180, 0), showCardDuration);
            transform.localScale = new Vector3(1, 1, 1);
            priceObj.SetActive(false);
        }

        public async void ShowPrice(bool value)
        {
            await UniTask.Delay((int)showCardDuration * 1000);
            priceObj.SetActive(value);
        }
    }
}