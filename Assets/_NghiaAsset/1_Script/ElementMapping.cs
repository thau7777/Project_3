using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Turnbase
{
    [System.Serializable]
    public struct ElementSpritePair
    {
        public CharacterElement element;
        public Sprite sprite;
    }

    [CreateAssetMenu(fileName = "ElementMapping", menuName = "Turnbase/Element Mapping")]
    public class ElementMapping : ScriptableObject
    {
        public List<ElementSpritePair> elementSprites;

        public Sprite GetElementSprite(CharacterElement element)
        {
            return elementSprites.FirstOrDefault(p => p.element == element).sprite;
        }
    }
}