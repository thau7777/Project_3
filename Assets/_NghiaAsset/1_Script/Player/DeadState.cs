using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Turnbase
{
    public class DeadState : BaseState
    {
        private const int DISSOLVE_EFFECT_INDEX = 0;

        public DeadState(CharacterStateMachine stateMachine) : base(stateMachine) { }

        public override void OnEnter()
        {
            Debug.Log($"{stateMachine.character.name} chuyển sang DeadState. Đã chết!");

            stateMachine.character.StopAllCoroutines();

            Collider col = stateMachine.character.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            ShaderEffectController shaderController = stateMachine.character.GetComponent<ShaderEffectController>();
            if (shaderController != null)
            {
                if (shaderController.GetVFXCount() > DISSOLVE_EFFECT_INDEX)
                {
                    shaderController.PlayEffect(DISSOLVE_EFFECT_INDEX);
                }
            }

            stateMachine.character.animator.Play("Die");

            stateMachine.character.StartCoroutine(HandleDeath());
        }

        private IEnumerator HandleDeath()
        {
            yield return new WaitForSeconds(3f);

            if (stateMachine.battleManager != null)
            {
                if (stateMachine.battleManager.activeCharacter == stateMachine.character)
                {
                    stateMachine.battleManager.activeCharacter = null;
                    stateMachine.battleManager.isProcessingTurn = false;
                }

                stateMachine.battleManager.RemoveCombatant(stateMachine.character);
            }

            stateMachine.character.gameObject.SetActive(false);
        }

        public override void OnExit()
        {

        }
    }
}