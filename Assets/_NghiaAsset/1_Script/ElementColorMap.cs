using UnityEngine;
using System.Collections.Generic;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "Element Color Map", menuName = "Skills/Element Color Map")]
    public class ElementColorMap : ScriptableObject
    {
        public List<ElementColorPair> colorMappings = new List<ElementColorPair>();

        [System.Serializable]
        public struct ElementColorPair
        {
            public ElementType elementType;
            public Color color;
        }

        public Color GetColor(ElementType element)
        {
            foreach (var pair in colorMappings)
            {
                if (pair.elementType == element)
                {
                    return pair.color;
                }
            }
            return Color.white;
        }
    }
}