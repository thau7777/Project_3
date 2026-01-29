using UnityEngine;
using System.Collections.Generic;
using System.Linq;



namespace Turnbase
{
    public class ReadyState : BaseState
    {
        private List<Character> enemies;
        private int currentIndex;

        private PlayerTurnBasedActions inputLogic;

        public ReadyState(CharacterStateMachine stateMachine, PlayerTurnBasedActions input) : base(stateMachine)
        {
            this.inputLogic = input;
        }
        public override void OnEnter()
        {
            stateMachine.character.animator.SetBool("IsIdle", true);
            Debug.Log(stateMachine.gameObject.name + " đã sẵn sàng hành động.");


            ShowTargetMarker(false);

            stateMachine.character.target = null;

            enemies = stateMachine.battleManager.allCombatants
                    .FindAll(c => !c.isPlayer && !c.isVirtualTracker && c.isAlive);
            if (enemies.Count > 0)
            {
                currentIndex = 0;
                stateMachine.character.target = enemies[currentIndex];

                RotateToTarget();

                if (stateMachine.character.isPlayer)
                {
                    if (enemies[currentIndex].targetMarker != null)
                    {
                        enemies[currentIndex].targetMarker.SetActive(true);
                    }
                }
            }
            else
            {
                stateMachine.character.target = null;
            }

            if (stateMachine.character.isPlayer && inputLogic != null)
            {
                inputLogic.LeftEvent += HandleMoveLeft;
                inputLogic.RightEvent += HandleMoveRight;
            }
        }

        private void HandleMoveLeft() => UpdateTarget(-1);
        private void HandleMoveRight() => UpdateTarget(1);

        public override void OnUpdate()
        {
            
        }

        private void UpdateTarget(int direction)
        {
            if (enemies.Count > 0)
            {
                if (stateMachine.character.target != null && stateMachine.character.target.targetMarker != null)
                {
                    stateMachine.character.target.targetMarker.SetActive(false);
                }

                currentIndex = (currentIndex + direction + enemies.Count) % enemies.Count;
                stateMachine.character.target = enemies[currentIndex];
                Debug.Log("Đã chuyển mục tiêu sang: " + enemies[currentIndex].gameObject.name + " tại vị trí slot: " + currentIndex);

                RotateToTarget();

                if (stateMachine.character.target != null && stateMachine.character.target.targetMarker != null)
                {
                    stateMachine.character.target.targetMarker.SetActive(true);
                }
            }
        }

        public override void OnExit()
        {
            if (stateMachine.character.isPlayer && inputLogic != null)
            {
                inputLogic.LeftEvent -= HandleMoveLeft;
                inputLogic.RightEvent -= HandleMoveRight;
            }
            ShowTargetMarker(false);
        }

        private void ShowTargetMarker(bool active)
        {
            if (enemies != null)
            {
                foreach (Character enemy in enemies)
                {
                    if (enemy != null && enemy.targetMarker != null)
                    {
                        enemy.targetMarker.SetActive(active);
                    }
                }
            }
        }

        private void RotateToTarget()
        {
            Character user = stateMachine.character;
            Character target = user.target;

            if (target != null)
            {
                Vector3 directionToTarget = target.transform.position - user.transform.position;
                directionToTarget.y = 0;

                if (directionToTarget.sqrMagnitude > 0)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                    user.transform.rotation = targetRotation;
                }
            }
        }
    }

}
