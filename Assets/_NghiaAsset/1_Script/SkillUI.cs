using Turnbase;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    public ElementColorMap elementColorMap;

    public void DisplaySkillDetails(Skill skillData)
    {
        Color skillColor = elementColorMap.GetColor(skillData.elementType);

    }
}