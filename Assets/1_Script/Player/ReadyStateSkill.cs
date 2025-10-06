using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ReadyStateSkill : BaseState
{
    private Skill selectedSkill;
    private List<Character> possibleTargets;
    private int currentIndex;

    public ReadyStateSkill(CharacterStateMachine stateMachine, Skill skill) : base(stateMachine)
    {
        this.selectedSkill = skill;
    }

    public override void OnEnter()
    {
        stateMachine.character.animator.SetBool("IsIdle", true);
        Debug.Log($"Entering ReadyStateSkill with skill: {selectedSkill.skillName}");

        ShowTargetMarker(false);
        stateMachine.character.target = null;

        switch (selectedSkill.targetType)
        {
            case SkillTargetType.Self:
                CameraAction.instance.TargetAllTeam();
                possibleTargets = new List<Character> { stateMachine.character };
                break;
            case SkillTargetType.Ally:
                CameraAction.instance.TargetAllTeam();
                possibleTargets = stateMachine.battleManager.allCombatants.FindAll(c => c != null && c.isPlayer && c.isAlive);
                break;
            case SkillTargetType.Enemy:
                possibleTargets = stateMachine.battleManager.allCombatants.FindAll(c => c != null && !c.isPlayer && c.isAlive);
                break;
            case SkillTargetType.Allies:
                CameraAction.instance.TargetAllTeam();
                possibleTargets = stateMachine.battleManager.allCombatants.FindAll(c => c != null && c.isPlayer && c.isAlive);

                break;
            case SkillTargetType.Enemies:
                CameraAction.instance.TargetAllEnemies();    
                possibleTargets = stateMachine.battleManager.allCombatants.FindAll(c => c != null && !c.isPlayer && c.isAlive);
                break;
        }

        if (possibleTargets.Count > 0)
        {
            if (selectedSkill.targetType == SkillTargetType.Enemies || selectedSkill.targetType == SkillTargetType.Allies)
            {
                foreach (Character character in possibleTargets)
                {
                    if (character != null)
                    {
                        if (character.targetMarker != null)
                        {
                            character.targetMarker.SetActive(true);
                        }
                        else
                        {
                            Debug.LogError($"Lỗi: Target marker bị thiếu trên nhân vật: {character.gameObject.name}. Vui lòng gán trong Inspector.");
                        }
                    }
                }
                stateMachine.character.target = null;
            }
            else
            {
                currentIndex = 0;
                stateMachine.character.target = possibleTargets[currentIndex];

                if (stateMachine.character.target != null && stateMachine.character.target.targetMarker != null)
                {
                    stateMachine.character.target.targetMarker.SetActive(true);
                }
                else
                {
                    Debug.LogError($"Lỗi: Target marker bị thiếu trên nhân vật: {stateMachine.character.target.gameObject.name}. Vui lòng gán trong Inspector.");
                }
            }
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy mục tiêu khả dụng cho kỹ năng: {selectedSkill.skillName}. Tự động hủy.");
            OnCancel();
        }
    }

    public override void OnUpdate()
    {
        if (stateMachine.character.isPlayer)
        {
            if (selectedSkill.targetType == SkillTargetType.Enemy || selectedSkill.targetType == SkillTargetType.Ally)
            {
                if (Input.GetKeyDown(KeyCode.A))
                    UpdateTarget(-1);
                if (Input.GetKeyDown(KeyCode.D))
                    UpdateTarget(1);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
                OnCancel();
        }
    }

    private void UpdateTarget(int direction)
    {
        if (possibleTargets.Count > 0)
        {
            if (stateMachine.character.target != null && stateMachine.character.target.targetMarker != null)
            {
                stateMachine.character.target.targetMarker.SetActive(false);
            }

            currentIndex = (currentIndex + direction + possibleTargets.Count) % possibleTargets.Count;
            stateMachine.character.target = possibleTargets[currentIndex];

            Debug.Log("Đã chuyển mục tiêu sang: " + stateMachine.character.target.gameObject.name + " tại vị trí slot: " + currentIndex);

            if (stateMachine.character.target != null && stateMachine.character.target.targetMarker != null)
            {
                stateMachine.character.target.targetMarker.SetActive(true);
            }
        }
    }

    public override void OnExit()
    {
        ShowTargetMarker(false);
    }

    private void ShowTargetMarker(bool active)
    {
        if (possibleTargets != null)
        {
            foreach (Character target in possibleTargets)
            {
                if (target != null && target.targetMarker != null)
                {
                    target.targetMarker.SetActive(active);
                }
            }
        }
    }

    public void OnConfirm()
    {
        if (selectedSkill.targetType == SkillTargetType.Enemies || selectedSkill.targetType == SkillTargetType.Allies)
        {
            stateMachine.character.target = possibleTargets.FirstOrDefault();
        }

        if (stateMachine.character.ownUI != null)
        {
            stateMachine.character.ownUI.PlayerSkillPanel.SetActive(false);
            stateMachine.character.ownUI.PlayerSummonPanel.SetActive(false);
            stateMachine.character.ownUI.confirmButton.gameObject.SetActive(false);
        }

        ShowTargetMarker(false);

        if (selectedSkill.skillType == SkillType.Summon)
        {
            GameObject petToSummon = null;

            if (selectedSkill.summonPrefab != null && selectedSkill.summonPrefab.Count > 0)
            {
                petToSummon = selectedSkill.summonPrefab.FirstOrDefault();
            }

            if (stateMachine.battleManager != null && petToSummon != null)
            {
                Debug.Log($"Xác nhận: Thực hiện Triệu hồi Pet '{petToSummon.name}'");

                stateMachine.battleManager.SummonPet(
                    stateMachine.character,
                    petToSummon 
                );

                stateMachine.battleManager.EndTurn(stateMachine.character);
            }
            else
            {
                Debug.LogError("Lỗi Triệu hồi: BattleManager bị thiếu hoặc danh sách Summon Prefab bị trống/null.");
                stateMachine.SwitchState(stateMachine.waitingState);
            }
        }
        else
        {
            Debug.Log($"Xác nhận: Chuyển sang AttackingState cho kỹ năng '{selectedSkill.skillName}'");
            stateMachine.SwitchState(new SkillAttackingState(stateMachine, selectedSkill));
        }
    }
    public void OnCancel()
    {
        if (stateMachine.character.ownUI != null)
        {
            stateMachine.character.ownUI.PlayerSkillPanel.SetActive(false);
            stateMachine.character.ownUI.confirmButton.gameObject.SetActive(false);
        }

        stateMachine.character.target = null;
        stateMachine.SwitchState(stateMachine.readyState);
    }

}
