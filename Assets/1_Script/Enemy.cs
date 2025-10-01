using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class Enemy : Character
{
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
            // Nếu không tìm thấy mục tiêu (người chơi đã chết hết), kết thúc trận chiến
            Debug.Log("Không tìm thấy người chơi để tấn công. Trận chiến kết thúc!");
            // TODO: Thêm logic kết thúc trận chiến ở đây
            stateMachine.battleManager.EndTurn(this);
        }
    }

    // Phương thức tìm kiếm mục tiêu người chơi
    private Character FindPlayerTarget()
    {
        // Lấy danh sách tất cả các nhân vật đang sống
        var aliveCombatants = battleManager.allCombatants.Where(c => c.isAlive).ToList();

        // Tìm tất cả người chơi trong danh sách đó
        List<Character> players = aliveCombatants.Where(c => c.isPlayer).ToList();

        if (players.Count > 0)
        {
            // Chọn một người chơi ngẫu nhiên từ danh sách
            int randomIndex = Random.Range(0, players.Count);
            return players[randomIndex];
        }
        return null;
    }
}
