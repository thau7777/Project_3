using UnityEngine;
using System.Collections;

public class TakingDamageState : BaseState
{
    public TakingDamageState(CharacterStateMachine stateMachine) : base(stateMachine) { }

    public override void OnEnter()
    {
        Debug.Log(stateMachine.gameObject.name + " đang bị nhận sát thương.");
        stateMachine.character.StartCoroutine(HandleTakingDamage());
    }

    private IEnumerator HandleTakingDamage()
    {
        stateMachine.character.animator.Play("Idle_Hurt");

        yield return new WaitForSeconds(0.5f);

        if (stateMachine.character.isAlive)
        {
            stateMachine.SwitchState(stateMachine.waitingState);
        }
        else
        {
            stateMachine.SwitchState(stateMachine.deadState);
        }
    }

    public override void OnExit()
    {
        stateMachine.character.StopAllCoroutines();
    }
}
