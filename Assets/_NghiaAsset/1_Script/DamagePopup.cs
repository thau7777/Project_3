using TMPro;
using UnityEngine;

namespace Turnbase
{
    public class DamagePopup : MonoBehaviour
    {
        public TextMeshProUGUI critText;
        public static DamagePopup Create(Vector3 position, int damageAmount, Transform parentTransform, Color color, bool isCrit) 
        {
            GameObject damagePopupObject = Instantiate(
                DamagePopupSpawn.i.pfdamagePopup,
                position,
                Quaternion.identity,
                parentTransform
            );

            DamagePopup damagePopup = damagePopupObject.GetComponent<DamagePopup>();

            if (damagePopup == null)
            {
                Destroy(damagePopupObject);
                return null;
            }

            damagePopup.Setup(damageAmount, color, isCrit); 
            Destroy(damagePopupObject, 1f);

            return damagePopup;
        }

        private TextMeshProUGUI damageText;


        private void Awake()
        {
            damageText = transform.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Setup(int damageAmount, Color color, bool isCrit)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = 1000; 
            }

            if (damageText != null)
            {
                damageText.color = color;
                damageText.text = damageAmount.ToString();
            }

            if (critText != null)
            {
                critText.gameObject.SetActive(isCrit);
            }
        }
    }

}