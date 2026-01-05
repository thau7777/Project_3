using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "RuneSO", menuName = "Scriptable Objects/RuneSO")]
    public class RuneSO : ScriptableObject
    {
        public int runeCount;
    }
}