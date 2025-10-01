using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public List<Character> allCombatants = new List<Character>();
    public Character activeCharacter;

    private bool isProcessingTurn = false;

    [Header("Players")]
    public Character[] playerPrefabs;
    public Transform[] playerSpawnPoints;

    [Header("Enemies")]
    public Transform[] enemySlots;
    public Character[] enemyPrefabs;

    public TurnOrderUI turnOrderUI;


    void Start()
    {
        SetupBattle();
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return null;  // đợi 1 frame
        StartCoroutine(UpdateActionGauge());
    }

    void SetupBattle()
    {
        allCombatants = new List<Character>();

        // Spawn Players
        int playerCount = Mathf.Min(playerPrefabs.Length, playerSpawnPoints.Length);
        for (int i = 0; i < playerCount; i++)
        {
            Character playerInstance = Instantiate(playerPrefabs[i], playerSpawnPoints[i].position, playerSpawnPoints[i].rotation);
            playerInstance.transform.SetParent(playerSpawnPoints[i]);

            playerInstance.isPlayer = true;
            allCombatants.Add(playerInstance);
            playerInstance.initialPosition = playerSpawnPoints[i].position;
            playerInstance.battleManager = this;

            CharacterStateMachine playerStateMachine = playerInstance.GetComponent<CharacterStateMachine>();
            if (playerStateMachine != null)
            {
                playerStateMachine.battleManager = this;
            }

            // 🔹 Gắn UI riêng cho mỗi player
            PlayerActionUI actionUI = playerInstance.GetComponentInChildren<PlayerActionUI>(true);
            if (actionUI != null)
            {
                // Gán owner để UI biết thuộc về player này
                actionUI.SetOwner(playerInstance);

                // Ẩn UI lúc spawn
                actionUI.Hide();

                // Subscribe parry event
                actionUI.OnParryAttempted += OnParryAttempted;

                // Lưu tham chiếu UI vào player
                playerInstance.ownUI = actionUI;
            }
        }

        // Spawn Enemies
        int enemyCount = Mathf.Min(enemySlots.Length, enemyPrefabs.Length);
        for (int i = 0; i < enemyCount; i++)
        {
            Character enemyInstance = Instantiate(enemyPrefabs[i], enemySlots[i].position, enemySlots[i].rotation);
            enemyInstance.transform.SetParent(enemySlots[i]);

            enemyInstance.isPlayer = false;
            allCombatants.Add(enemyInstance);
            enemyInstance.initialPosition = enemySlots[i].position;
            enemyInstance.battleManager = this;

            CharacterStateMachine enemyStateMachine = enemyInstance.GetComponent<CharacterStateMachine>();
            if (enemyStateMachine != null)
            {
                enemyStateMachine.battleManager = this;
            }
        }

        // Reset state ban đầu
        foreach (Character combatant in allCombatants)
        {
            if (combatant.stateMachine != null)
            {
                combatant.stateMachine.SwitchState(combatant.stateMachine.waitingState);
                combatant.actionGauge = 0;
            }
        }
    }

    private IEnumerator UpdateActionGauge()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            if (activeCharacter == null && !isProcessingTurn)
            {
                bool someoneReady = false;
                foreach (var combatant in allCombatants)
                {
                    if (combatant.isAlive)
                    {
                        combatant.actionGauge += combatant.stats.agility * Time.deltaTime;

                        if (combatant.actionGauge >= 100 && combatant.stateMachine.currentState is WaitingState)
                        {
                            someoneReady = true;
                        }
                    }
                }

                if (turnOrderUI != null)
                {
                    turnOrderUI.UpdateActionGaugeUI(allCombatants);
                }

                if (someoneReady)
                {
                    isProcessingTurn = true;
                    var readyCharacters = allCombatants
                        .Where(c => c.actionGauge >= 100 && c.isAlive)
                        .OrderByDescending(c => c.actionGauge)
                        .ToList();

                    if (readyCharacters.Any())
                    {
                        AdvanceTurn(readyCharacters.First());
                    }
                }
            }
            yield return null;
        }
    }
 
    public void AdvanceTurn(Character characterToAct)
    {
        if (activeCharacter != null) return;

        activeCharacter = characterToAct;
        Debug.Log($"Đến lượt: {activeCharacter.gameObject.name}");

        if (activeCharacter.isPlayer)
        {
            foreach (var player in allCombatants.Where(c => c.isPlayer))
            {
                if (player.ownUI != null) player.ownUI.Hide();
            }

            if (activeCharacter.ownUI != null)
            {
                activeCharacter.ownUI.ShowUI();
                activeCharacter.ownUI.SetupSkillUI(activeCharacter.skills);
                activeCharacter.ownUI.SetActiveCharacter(activeCharacter);
            }
        }
        else
        {
            StartCoroutine(EnemyTurn(activeCharacter));
        }

        StartCoroutine(DelayedStartTurn(activeCharacter));

        activeCharacter.stateMachine.SwitchState(activeCharacter.stateMachine.waitingState);

        if (turnOrderUI != null)
        {
            turnOrderUI.HighlightActiveCharacter(activeCharacter);
        }

        
        
    }

    private IEnumerator DelayedStartTurn(Character character)
    {
        yield return new WaitForSeconds(2f);
        CameraAction.instance.LookCameraAtTarget(character);
    }


    private IEnumerator EnemyTurn(Character enemy)
    {
        Debug.Log("Đến lượt của kẻ địch: " + enemy.gameObject.name);



        yield return new WaitForSeconds(1f);

        Enemy enemyComponent = enemy.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.PerformTurn();
        }

        // Enemy đã set target trong PerformTurn()
        if (enemy.target != null)
        {
            StartCoroutine(EnemyParryWindow(enemy, enemy.target));
        }
    }


    private IEnumerator EnemyParryWindow(Character enemy, Character target)
    {
        if (target == null || !target.isAlive) yield break;

        float parryTimer = 0f;
        float attackDuration = 1.5f;

        // Chỉ hiện UI của target
        if (target.ownUI != null)
            target.ownUI.ShowParryUI(true);

        while (parryTimer < attackDuration)
        {
            parryTimer += Time.deltaTime;
            float normalizedValue = parryTimer / attackDuration; // Giá trị từ 0.0f đến 1.0f

            if (target.ownUI != null)
            {
                // Cập nhật thanh đổ đầy Image
                target.ownUI.UpdateParryFill(normalizedValue);
            }

            // Logic kiểm tra Cửa sổ Parry
            bool isInParryWindow = (normalizedValue >= 0.6f && normalizedValue <= 0.9f);

            if (isInParryWindow)
            {
                if (!target.isParryable)
                {
                    target.isParryable = true;
                    // ⬅️ CHUYỂN SPRITE SANG TRẠNG THÁI READY
                    if (target.ownUI != null)
                        target.ownUI.SetParrySprite(true);
                }
            }
            else
            {
                if (target.isParryable)
                {
                    target.isParryable = false;
                    // ⬅️ CHUYỂN SPRITE VỀ TRẠNG THÁI MẶC ĐỊNH
                    if (target.ownUI != null)
                        target.ownUI.SetParrySprite(false);
                }
            }

            yield return null;
        }

        // Khi Coroutine kết thúc (thời gian đã hết)
        if (target.ownUI != null)
        {
            target.ownUI.ShowParryUI(false);
            // ⬅️ Đảm bảo Sprite trở về mặc định nếu đang ở ngưỡng (và tránh lỗi)
            target.ownUI.SetParrySprite(false);
        }
        target.isParryable = false; // Đảm bảo trạng thái Parry bị tắt
    }


    public void OnParryAttempted()
    {
        // Enemy đang đánh target nào thì target đó mới được parry
        if (activeCharacter != null && activeCharacter is Enemy enemy)
        {
            Character target = enemy.target;
            if (target != null && target.isParryable)
            {
                Debug.Log("Parry thành công!");
                target.isParryable = false;

                activeCharacter.stateMachine.SwitchState(activeCharacter.stateMachine.interruptedState);
                target.stateMachine.SwitchState(target.stateMachine.parryingState);
            }
            else
            {
                Debug.Log("Parry không thành công!");
            }
        }
    }


    public void EndTurn(Character character)
    {
        if (character == activeCharacter)
        {

            EventBus<ShowPanelEvent>.Raise(new ShowPanelEvent(panelName: "EnemyUI"));


            activeCharacter = null;
            if (character.stateMachine != null)
            {
                character.stateMachine.SwitchState(character.stateMachine.waitingState);
                character.actionGauge = 0;
            }
            isProcessingTurn = false;
        }
    }

    public void RemoveCombatant(Character character)
    {
        if (allCombatants.Contains(character))
        {
            allCombatants.Remove(character);
        }
    }


    

}
