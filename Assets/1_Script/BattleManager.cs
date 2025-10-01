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

    private Coroutine currentParryWindow;


    void Start()
    {
        SetupBattle();
        StartCoroutine(DelayedStart());
    }

    void Update()
    {
        // 1. Chỉ cho phép kiểm tra input khi đang là lượt của kẻ địch và cửa sổ Parry đang mở
        if (activeCharacter != null && !activeCharacter.isPlayer && currentParryWindow != null)
        {
            // 2. Tìm mục tiêu (player) mà kẻ địch đang nhắm đến
            Enemy enemy = activeCharacter as Enemy;
            Character playerTarget = enemy?.target;

            // 3. Kiểm tra phím Space và trạng thái có thể Parry
            if (playerTarget != null && playerTarget.isParryable && Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Nhấn Space: Bắt đầu cố gắng Parry!");

                // Gọi hàm xử lý Parry chính
                OnParryAttempted();
            }
        }
    }

    IEnumerator DelayedStart()
    {
        yield return null;  
        StartCoroutine(UpdateActionGauge());
    }

    void SetupBattle()
    {
        allCombatants = new List<Character>();

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

            PlayerActionUI actionUI = playerInstance.GetComponentInChildren<PlayerActionUI>(true);
            if (actionUI != null)
            {
                actionUI.SetOwner(playerInstance);

                actionUI.Hide();

                actionUI.OnParryAttempted += OnParryAttempted;

                playerInstance.ownUI = actionUI;
            }
        }

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

        if (enemy.target != null)
        {
            float startTime = 0f;
            float endTime = 0f;

            yield return new WaitUntil(() => enemy.isAttackReadyForParry == true);
            startTime = Time.time;
            enemy.isAttackReadyForParry = false; 
            enemy.isParryWindowFinished = false; 


            yield return new WaitUntil(() => enemy.isParryWindowFinished == true);
            endTime = Time.time;
            enemy.isParryWindowFinished = false; 

            float duration = endTime - startTime;
            enemy.parryWindowDuration = duration;
            Debug.Log($"Thời lượng Parry tính toán: {duration}s");

            StartParryWindow(enemy, enemy.target, duration);
        }
    }

    public void StartParryWindow(Character enemy, Character target, float duration)
    {
        if (currentParryWindow != null)
        {
            StopCoroutine(currentParryWindow);
        }

        currentParryWindow = StartCoroutine(ImmediateParryWindow(enemy, target, duration));
    }


    private IEnumerator ImmediateParryWindow(Character enemy, Character target, float duration)
    {
        if (target == null || !target.isAlive) yield break;

        float parryTimer = 0f;

        if (target.ownUI != null)
        {
            target.ownUI.ShowParryUI(true);
            target.ownUI.SetParrySprite(true); 
        }
        target.isParryable = true; 

        while (parryTimer < duration)
        {
            parryTimer += Time.deltaTime;
            float normalizedValue = Mathf.Clamp01(parryTimer / duration);

            if (target.ownUI != null)
            {
                target.ownUI.UpdateParryFill(normalizedValue);
            }

            yield return null;
        }


        if (target.ownUI != null)
        {
            target.ownUI.ShowParryUI(false);
            target.ownUI.SetParrySprite(false);
        }
        target.isParryable = false;
        currentParryWindow = null;

    }


    public void OnParryAttempted()
    {
        if (activeCharacter != null && activeCharacter is Enemy enemy)
        {
            Character target = enemy.target;
            if (target != null && target.isParryable)
            {
                Debug.Log("Parry thành công!");
                target.isParryable = false;

                if (currentParryWindow != null)
                {
                    StopCoroutine(currentParryWindow);
                    currentParryWindow = null;
                    if (target.ownUI != null)
                    {
                        target.ownUI.ShowParryUI(false);
                        target.ownUI.SetParrySprite(false);
                    }
                }


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