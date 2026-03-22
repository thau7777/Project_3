using UnityEngine;

namespace MyRule
{
    public enum AchievementType
    {
        KillEnemy,
        CollectSigil,
        Discovery,
    }

    [CreateAssetMenu(fileName = "AchievementConfig", menuName = "Scriptable Objects/AchievementConfig")]
    public class AchievementConfig : ScriptableObject
    {
        [Header("Info")]
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
        public string achievementName;
        public AchievementType type;
        [ShowIfEnumValue("type", AchievementType.KillEnemy)] public int targetValue;
        [ShowIfEnumValue("type", AchievementType.CollectSigil)] public string targetSigilName;
        [ShowIfEnumValue("type", AchievementType.Discovery)] public EMap map;

        [Header("Reward")]
        public int goldReward;
        public string sigilRewardId;
    }
}