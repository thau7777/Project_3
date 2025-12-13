using UnityEngine;
using System.Collections.Generic;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "NewCharcterClassProfile", menuName = "Character/Class Profile")]
    public class CharacterClassProfile : ScriptableObject
    {
        public CharacterClass characterClass;
        public RuntimeAnimatorController animatorController;
        [ShowIfEnumValue("characterClass", CharacterClass.Magical,
                                           CharacterClass.Sword_Shield,
                                           CharacterClass.Summon,
                                           CharacterClass.Tank)]
        public List<Skill> initialSkills;

    }

}



