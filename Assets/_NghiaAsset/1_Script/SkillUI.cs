using Turnbase;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    public ElementColorMap elementColorMap;

    public void DisplaySkillDetails(Skill skillData)
    {
        Color skillColor = elementColorMap.GetColor(skillData.elementType);

        // 2. Áp dụng màu (Ví dụ: cho TextMeshProUGUI)
        // mySkillNameText.color = skillColor;

        // 3. Hoặc truyền màu vào hệ thống Damage Popup
        //DamagePopup.Create(position, damage, parent, skillColor);
    }
}