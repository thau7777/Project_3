using System.Collections;
using UnityEngine;

public class AttackingState : BaseState
{
    private Character target;


    public AttackingState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        target = stateMachine.character.target;

        if (target == null || !target.isAlive)
        {
            Debug.LogWarning("Mục tiêu không hợp lệ hoặc đã chết. Trở lại trạng thái chờ.");
            stateMachine.battleManager.EndTurn(stateMachine.character);
            return;
        }

        Skill basicAttack = ScriptableObject.CreateInstance<Skill>();
        basicAttack.skillName = "Basic Attack";
        basicAttack.damage = stateMachine.character.stats.attack;
        basicAttack.skillType = SkillType.Damage;

        ICommand command;

        switch (stateMachine.character.characterClass)
        {
            case CharacterClass.Sword_Shield:
                command = new AttackCommand(stateMachine.character, target, basicAttack);
                break;
            case CharacterClass.Magical:
                command = new StationaryAttackCommand(stateMachine.character, target, basicAttack);
                break;
            case CharacterClass.Summon:
                command = new StationaryAttackCommand(stateMachine.character, target, basicAttack);
               break;
            case CharacterClass.Enemy:
                command = new AttackCommand(stateMachine.character, target, basicAttack);
                break;
            default:
                command = new StationaryAttackCommand(stateMachine.character, target, basicAttack);
                break;
        }


        stateMachine.character.StartCoroutine(ExecuteCommand(command));


    }

    private IEnumerator ExecuteCommand(ICommand command)
    {
        yield return stateMachine.character.StartCoroutine(command.Execute());
        stateMachine.battleManager.EndTurn(stateMachine.character);
    }

    public override void OnUpdate() { }

    public override void OnExit()
    {
        stateMachine.character.StopAllCoroutines();
    }
}