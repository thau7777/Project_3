using TMPro;
using UnityEngine;


namespace Turnbase
{
    public struct StatData
    {
        public string Name;
        public string Value;
        public string Suffix; 
    }

    public class StatEntry : MonoBehaviour
    {
        public TextMeshProUGUI StatNameText;

        public TextMeshProUGUI StatValueText;

        public void Setup(string name, string value)
        {
            StatNameText.text = name;
            StatValueText.text = value;
        }
    }

}

