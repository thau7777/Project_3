using System.Collections.Generic;
using UnityEngine;

namespace Turnbase
{
    public class ReadyStateItem : BaseState
    {
        private Tb_Item selectedItem;
        private List<Character> possibleTargets;
        private int currentIndex;

        public Tb_Item SelectedItem { get; private set; }

        public ReadyStateItem(CharacterStateMachine stateMachine, Tb_Item item) : base(stateMachine)
        {
            this.selectedItem = item;   
            this.SelectedItem = item; 
        }

        public override void OnEnter()
        {
            stateMachine.character.animator.SetBool("IsIdle", true);
            Debug.Log($"Entering ReadyStateItem với vật phẩm: {selectedItem.itemName}");

            ShowTargetMarker(false);
            stateMachine.character.target = null;

            possibleTargets = stateMachine.battleManager.allCombatants
                .FindAll(c => c != null && c.isPlayer && c.isAlive && !c.isVirtualTracker);

            if (possibleTargets.Count > 0)
            {
                currentIndex = 0;
                stateMachine.character.target = possibleTargets[currentIndex];

                if (stateMachine.character.target.targetMarker != null)
                {
                    stateMachine.character.target.targetMarker.SetActive(true);
                }

                CameraAction.instance.TargetAllTeam();
            }
            else
            {
                Debug.LogWarning("Không có mục tiêu hợp lệ để sử dụng vật phẩm.");
                OnCancel();
            }
        }

        public override void OnUpdate()
        {
            if (stateMachine.character.isPlayer)
            {
                if (Input.GetKeyDown(KeyCode.A))
                    UpdateTarget(-1);
                if (Input.GetKeyDown(KeyCode.D))
                    UpdateTarget(1);

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

                if (stateMachine.character.target != null && stateMachine.character.target.targetMarker != null)
                {
                    stateMachine.character.target.targetMarker.SetActive(true);
                }

                Debug.Log($"Đã chuyển mục tiêu vật phẩm sang: {stateMachine.character.target.name}");
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
            Debug.Log($"Xác nhận sử dụng {selectedItem.itemName} lên {stateMachine.character.target.name}");

            if (stateMachine.character.ownUI != null)
            {
                stateMachine.character.ownUI.PlayerItemPanel.SetActive(false);
                stateMachine.character.ownUI.confirmButton.gameObject.SetActive(false);
            }

            ShowTargetMarker(false);

            stateMachine.SwitchState(stateMachine.waitingState);

            ICommand itemCmd = new UseItemCommand(stateMachine.character, stateMachine.character.target, selectedItem);
            stateMachine.StartCoroutine(itemCmd.Execute());
        }

        public void OnCancel()
        {
            if (stateMachine.character.ownUI != null)
            {
                stateMachine.character.ownUI.PlayerItemPanel.SetActive(true);
                stateMachine.character.ownUI.confirmButton.gameObject.SetActive(false);
            }

            ShowTargetMarker(false);
            stateMachine.character.target = null;

            stateMachine.SwitchState(stateMachine.readyState);
        }
    }
}