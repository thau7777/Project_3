using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MyRule
{
    public class LobbyShopProductView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI productName;
        [SerializeField] private Image unitIcon;
        [SerializeField] private Sprite goldImg;
        [SerializeField] private Sprite crystalImg;
        [SerializeField] private Sprite realMoneyImg;
        [SerializeField] private TextMeshProUGUI pricesTxt;
        [SerializeField] private LobbyShopProductConfig productConfig;
        [SerializeField] private Button button;

        private void Start()
        {
            button.onClick.AddListener(OnClick);
        }

        public void SetProduct(LobbyShopProductConfig productConfig)
        {
            this.productConfig = productConfig;
            SetProduct();
        }

        public void SetProduct()
        {
            if (productConfig == null) return;

            switch (productConfig.productType)
            {
                case EProduct.Card:
                    {
                        icon.material = productConfig.cardConfig.cardFoil;
                        break;
                    }
                case EProduct.Sigil:
                    {
                        icon.sprite = productConfig.sigilSO.sigilIcon;
                        break;
                    }
                default:
                    {
                        icon.sprite = productConfig.icon;
                        break;
                    }
            }
            productName.text = productConfig.productName;

            switch (productConfig.unit)
            {
                case EUnit.Gold:
                    unitIcon.sprite = goldImg;
                    break;
                case EUnit.Crystal:
                    unitIcon.sprite = crystalImg;
                    break;
                case EUnit.RealMoney:
                    unitIcon.sprite = realMoneyImg;
                    break;
            }

            pricesTxt.text = productConfig.prices.ToString();
        }

        public void OnClick()
        {
            switch (productConfig.productType)
            {
                case EProduct.Sigil:
                    {
                        if (LobbyShopManager.Instance.BuySigil(productConfig.unit, productConfig.prices, productConfig))
                        {
                            gameObject.SetActive(false);
                        }

                        break;
                    }
                case EProduct.Card:
                    {
                        if (LobbyShopManager.Instance.BuyCard(productConfig.unit, productConfig.prices, productConfig))
                        {
                            gameObject.SetActive(false);
                        }

                        break;
                    }
                case EProduct.Gold:
                    {
                        if (LobbyShopManager.Instance.BuyGold(productConfig.prices, productConfig.gold))
                        {

                        }

                        break;
                    }
                case EProduct.Crystal:
                    {
                        if (LobbyShopManager.Instance.BuyCrystal(0, productConfig.crystal))
                        {

                        }

                        break;
                    }
            }
        }
    }
}