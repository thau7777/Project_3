using System.Collections;
using UnityEngine;



namespace Turnbase
{
    public abstract class BattleRule : ScriptableObject
    {
        [Header("Thông tin Luật")]
        public string ruleName = "Luật Trận Đấu Mới";
        [TextArea(3, 5)]
        public string description = "Mô tả luật lệ này ảnh hưởng đến trận đấu như thế nào.";

        public abstract IEnumerator ExecuteRule(BattleManager battleManager);

        public virtual void ResetRule(BattleManager battleManager)
        {
            // Logic dọn dẹp 
        }
    }
}