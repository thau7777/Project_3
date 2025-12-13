using System.Collections;
using UnityEngine;

namespace Turnbase
{
    public enum TargetScope
    {
        AllCombatants,
        Players,
        Enemies
    }

    public abstract class BattleRule : ScriptableObject
    {
        [Header("Thông tin Luật")]
        public string ruleName = "Luật Trận Đấu Mới";
        [TextArea(3, 5)]
        public string description = "Mô tả luật lệ này ảnh hưởng đến trận đấu như thế nào.";

        [Header("Cấu hình Mục tiêu")]
        public TargetScope targetScope = TargetScope.AllCombatants;

        public abstract IEnumerator ExecuteRule(BattleManager battleManager, Character characterToAct);

        public virtual void ResetRule(BattleManager battleManager)
        {
        }
    }
}