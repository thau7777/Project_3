using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace Turnbase
{
    [Serializable]
    public struct ElementInteraction
    {
        public ElementType AttackerElement;

        public CharacterElement DefenderElement;

        public float DamageMultiplier;
    }

    [CreateAssetMenu(fileName = "ElementChart", menuName = "Turnbase/Element Chart")]
    public class ElementChart : ScriptableObject
    {
        public List<ElementInteraction> Interactions; 

        public float GetMultiplier(ElementType attackElement, CharacterElement defenseElement)
        {
            const float DefaultMultiplier = 1.0f;

            var interaction = Interactions.FirstOrDefault(
                i => i.AttackerElement == attackElement && i.DefenderElement == defenseElement
            );

            if (interaction.DamageMultiplier != 0) 
            {
                return interaction.DamageMultiplier;
            }

            return DefaultMultiplier;
        }
    }
}