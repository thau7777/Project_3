using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    private Character currentCharacter;


    public void Animation_ReadyParry()
    {
        isAttackReadyForParry = true;
    }

    public void Animation_EndParry()
    {
        isParryWindowFinished = true;
    }

    public void PerformTurn()
    {
        Character playerTarget = FindPlayerTarget();

        if (playerTarget != null)
        {
            this.target = playerTarget;

            CameraAction.instance.LookCameraAtTarget(this.target);

            Debug.Log(gameObject.name + " đã tìm thấy mục tiêu: " + playerTarget.gameObject.name);

            stateMachine.SwitchState(stateMachine.attackingState);

            EventBus<HidePanelEvent>.Raise(new HidePanelEvent(panelName: "EnemyUI"));

        }
        else
        {
            Debug.Log("Không tìm thấy người chơi để tấn công. Trận chiến kết thúc!");
            stateMachine.battleManager.EndTurn(this);
        }
    }

    private Character FindPlayerTarget()
    {
        var aliveCombatants = battleManager.allCombatants.Where(c => c.isAlive).ToList();

        List<Character> players = aliveCombatants.Where(c => c.isPlayer).ToList();

        if (players.Count > 0)
        {
            int randomIndex = Random.Range(0, players.Count);
            return players[randomIndex];
        }
        return null;
    }
}
