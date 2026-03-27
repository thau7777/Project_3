using UnityEngine;

namespace MyRule
{
    [CreateAssetMenu(fileName = "CardConfig", menuName = "Scriptable Objects/CardConfig")]
    public class CardConfig : ScriptableObject
    {
        [SerializeField] public string id;
#if UNITY_EDITOR
        [ContextMenu("Generate New ID")]
        public void GenerateNewID()
        {
            id = System.Guid.NewGuid().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
        public Material cardFoil;
    }
}