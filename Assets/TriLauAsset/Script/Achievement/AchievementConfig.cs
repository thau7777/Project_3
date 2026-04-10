using UnityEngine;

namespace MyRule
{
    public enum AchievementType
    {
        KillEnemy,
        CollectSigil,
        Discovery,
        Basic,
    }

    public enum RewardType
    {
        None,
        Gold,
        Crystal,
        Sigil,
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
        public Sprite icon;
        public AchievementType type;
        public int targetValue;
        [ShowIfEnumValue("type", AchievementType.Discovery)] public EMap targetMap;
        [ShowIfEnumValue("type", AchievementType.CollectSigil)] public SigilSO targetSigil;

        [Header("Reward")]
        public RewardType rewardType;
        [ShowIfEnumValue("rewardType", RewardType.Gold)] public int goldReward;
        [ShowIfEnumValue("rewardType", RewardType.Crystal)] public int crystalReward;
        [ShowIfEnumValue("rewardType", RewardType.Sigil)] public SigilSO sigilReward;
    }
}