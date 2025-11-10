using TMPro;
using UnityEngine;

namespace Turnbase
{
    public class DamagePopup : MonoBehaviour
    {
        public static DamagePopup Create(Vector3 position, int damageAmount, Transform parentTransform, Color color) // <-- THÊM THAM SỐ COLOR
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

            // Truyền màu sắc vào Setup
            damagePopup.Setup(damageAmount, color); // <-- TRUYỀN COLOR VÀO SETUP

            // Hủy Popup sau 2 giây (đã thảo luận trước đó)
            Destroy(damagePopupObject, 2f);

            return damagePopup;
        }

        private TextMeshProUGUI damageText;


        private void Awake()
        {
            damageText = transform.GetComponentInChildren<TextMeshProUGUI>();
        }

        public void Setup(int damageAmount, Color color) // <-- CẬP NHẬT SETUP ĐỂ NHẬN COLOR
        {
            if (damageText != null)
            {
                // ÁP DỤNG MÀU LÊN TEXT
                damageText.color = color;
                damageText.text = damageAmount.ToString();
            }
        }
    }

}