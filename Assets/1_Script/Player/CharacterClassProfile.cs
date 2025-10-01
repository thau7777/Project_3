using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewCharcterClassProfile", menuName = "Character/Class Profile")]
public class CharacterClassProfile : ScriptableObject
{
    public CharacterClass characterClass;
    public  RuntimeAnimatorController animatorController;
    public List<Skill> initialSkills;

}

