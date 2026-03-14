using UnityEngine;
using Turnbase;

namespace Turnbase
{
    public class RoundTracker : Character
    {
        [Header("Thông số Vòng đấu")]
        public int currentRound = 0;
        public int VIRTUAL_AGILITY = 100;

        void Awake()
        {
            this.isVirtualTracker = true;
            this.isPlayer = false;

            stateMachine = GetComponent<CharacterStateMachine>();
            battleManager = FindFirstObjectByType<BattleManager>();


            info = new CharacterInfo { name = "END OF ROUND", Avatar = null, level = 0 };

            stats = new CharacterStats
            {
                speed = VIRTUAL_AGILITY,
                maxHP = 1,
                currentHP = 1
            };
            isVirtualTracker = true;
            isPlayer = false;
            actionGauge = 0; 

            skills = new System.Collections.Generic.List<Skill>();
            passiveSkills = new System.Collections.Generic.List<SkillPassive>();
            buffManager = null;
            debuffManager = null;
            animator = null;

            Debug.Log("[RoundTracker] Đối tượng kiểm soát vòng đấu đã khởi tạo.");

        }

        public void ExecuteRoundPhase()
        {
            Debug.Log($"--- BẮT ĐẦU GIAI ĐOẠN: {info.name} ---");
            if(battleManager != null)
            {
                if(battleManager.availableRules == null)
                {
                    battleManager.EndTurn(this);
                }
            }
        }
    }
}