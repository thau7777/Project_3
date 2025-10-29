using System.Collections;
using UnityEngine;



namespace Turnbase
{
    public class SkillAttackingState : BaseState
    {
        private Skill selectedSkill;

        public SkillAttackingState(CharacterStateMachine stateMachine, Skill skill) : base(stateMachine)
        {
            selectedSkill = skill;
        }

        public override void OnEnter()
        {
            Character user = stateMachine.character;
            Character target = user.target;

            ICommand command = SkillCommandFactory.CreateCommand(
                user,
                target,
                selectedSkill,
                stateMachine.battleManager
            );

            if (command != null)
            {
                user.StartCoroutine(ExecuteCommand(command));
            }
            else
            {
                Debug.Log("Không có command phù hợp, kết thúc lượt.");
                stateMachine.battleManager.EndTurn(user);
            }

        }


        private IEnumerator ExecuteCommand(ICommand command)
        {
            yield return command.Execute();

            stateMachine.battleManager.EndTurn(stateMachine.character);
        }

        public override void OnUpdate() { }

        public override void OnExit()
        {
            stateMachine.character.StopAllCoroutines();

        }
    }

}
