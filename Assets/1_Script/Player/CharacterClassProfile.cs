using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class AppearanceSetting
{
    public string helmetName;
    public bool showHelmet;
    // ... thêm các trường khác nếu cần
}

[CreateAssetMenu(fileName = "NewCharcterClassProfile", menuName = "Character/Class Profile")]
public class CharacterClassProfile : ScriptableObject
{
    public CharacterClass characterClass;
    public  RuntimeAnimatorController animatorController;
    public List<Skill> initialSkills;

    [Header("Appearance")]
    public AppearanceSetting classAppearance;
}

