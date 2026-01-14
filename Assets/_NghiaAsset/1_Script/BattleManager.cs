using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

namespace Turnbase
{
    public class BattleManager : MonoBehaviour
    {
        public List<Character> allCombatants = new List<Character>();

        public Character activeCharacter;

        public bool isProcessingTurn = false;

        [Header("Players")]
        public Character[] playerPrefabs;
        public Transform[] playerSpawnPoints;

        [Header("Enemies")]
        public Transform[] enemySlots;
        public EnemyEncounter encounterToLoad;

        public TurnOrderUI turnOrderUI;

        public BattleUIManager uiManager;

        public ElementChart elementChart;

        private Coroutine currentParryWindow;

        public CharacterStatUI statDisplayPanel;

        public BattleBuffManager turnbuffManager;

        private int currentWaveIndex = 0;
        private bool isActionGaugeRunning = false;

        [Header("Battle Rules")]
        public List<BattleRule> availableRules;
        private BattleRule currentRule = null;

        [Header("Round Tracking")]
        public RoundTracker roundTrackerPrefab;

        [Header("Round Limit")]
        public int startRounds = 5;
        private RoundTracker instantiatedRoundTracker;

        [Header("Cinematics")]
        public PlayableDirector mainDirector;

        public ParryMiniGame parryUI;

        public EvadeMiniGame evadeUI;

        public BattleSpawner spawner;

        public bool isMiniGameRunning = false;

        public BattleTurnHandler turnHandler;

        public bool isAnimBased = false;

        void Start()
        {
            if (spawner != null) spawner.Initialize(this);
            if (turnHandler != null) turnHandler.Initialize(this);

            SetupBattle();

            if (uiManager != null)
                uiManager.InitializeCombatantButtons(allCombatants, statDisplayPanel, this);

            ChooseRandomRule();

            if (turnHandler != null)
                StartCoroutine(turnHandler.UpdateActionGaugeRoutine());
        }

        void Update()
        {

        }

        void SetupBattle()
        {
            allCombatants.Clear();

            int playerCount = Mathf.Min(playerPrefabs.Length, playerSpawnPoints.Length);
            for (int i = 0; i < playerCount; i++)
            {
                spawner.SpawnCombatant(playerPrefabs[i].gameObject, true, playerSpawnPoints[i].position);
            }

            if (roundTrackerPrefab != null)
            {
                RoundTracker trackerInstance = Instantiate(roundTrackerPrefab, Vector3.zero, Quaternion.identity);
                trackerInstance.battleManager = this;
                trackerInstance.stats.currentHP = trackerInstance.stats.maxHP;
                trackerInstance.isVirtualTracker = true;
                allCombatants.Add(trackerInstance);
                this.instantiatedRoundTracker = trackerInstance;
            }

            currentWaveIndex = 0;
            if (encounterToLoad != null && encounterToLoad.waves.Length > 0)
            {
                if (uiManager != null) uiManager.UpdateWaveDisplay(currentWaveIndex + 1, encounterToLoad.waves.Length);

                spawner.SpawnWaveImmediately(currentWaveIndex, encounterToLoad, enemySlots);
            }

            if (turnOrderUI != null) turnOrderUI.UpdateActionGaugeUI(allCombatants);
        }

        public void CheckWaveCondition()
        {
            var livingEnemies = allCombatants.Where(c => !c.isPlayer && c.isAlive && !c.isVirtualTracker).ToList();

            if (livingEnemies.Count == 0)
            {
                currentWaveIndex++;
                if (encounterToLoad != null && currentWaveIndex < encounterToLoad.waves.Length)
                {
                    allCombatants.RemoveAll(c => c != null && !c.isPlayer && !c.isAlive);
                    if (uiManager != null) uiManager.UpdateWaveDisplay(currentWaveIndex + 1, encounterToLoad.waves.Length);

                    isProcessingTurn = true;
                    if (turnHandler != null) turnHandler.isProcessingTurn = true;

                    StartCoroutine(HandleNewWaveTransition());
                }
                else
                {
                    CheckWinCondition(true);
                }
            }
            else
            {
                var livingPlayers = allCombatants.Where(c => c.isPlayer && c.isAlive).ToList();
                if (livingPlayers.Count == 0) TB_Menu.instance.ShowLoseMenu();
            }
        }

        private IEnumerator HandleNewWaveTransition()
        {
            foreach (var c in allCombatants)
            {
                c.isAttackBlocked = false;
                c.isParrySuccessful = false;
                c.isParryable = false;
            }

            yield return StartCoroutine(spawner.SpawnWaveRoutine(currentWaveIndex, encounterToLoad, enemySlots));

            yield return new WaitForEndOfFrame();

            isProcessingTurn = false;
            if (turnHandler != null) turnHandler.isProcessingTurn = false;
        }

        public IEnumerator AdvanceTurn(Character characterToAct)
        {
            if (activeCharacter != null) yield break;

            activeCharacter = characterToAct;
            Debug.Log($"<color=yellow>[TURN]</color> Đến lượt: {activeCharacter.name}");

            if (activeCharacter.isAlive && activeCharacter.stats != null)
            {
                activeCharacter.stats.currentMP = Mathf.Min(activeCharacter.stats.currentMP + 20, activeCharacter.stats.maxMP);
                uiManager.UpdateAllCharacterUIs(allCombatants);
            }

            if (activeCharacter is RoundTracker roundTracker)
            {
                roundTracker.ExecuteRoundPhase();
                if (currentRule != null)
                {
                    yield return StartCoroutine(currentRule.ExecuteRule(this, activeCharacter));
                    yield return new WaitForSeconds(1.5f);
                }

                if (roundTracker.currentRound <= -1)
                {
                    TB_Menu.instance.ShowLoseMenu();
                    isProcessingTurn = true;
                    yield break;
                }
                EndTurn(roundTracker);
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
            CameraAction.instance.LookCameraAtTarget(activeCharacter);
            if (turnOrderUI != null) turnOrderUI.HighlightActiveCharacter(activeCharacter);

            if (!activeCharacter.isAlive) { EndTurn(activeCharacter); yield break; }

            turnbuffManager.ProcessPassiveSkills(activeCharacter);
            if (activeCharacter.buffManager != null) activeCharacter.buffManager.ProcessTurnStartDecay();
            if (activeCharacter.debuffManager != null)
            {
                yield return StartCoroutine(activeCharacter.debuffManager.ApplyDoTDamage());
                activeCharacter.debuffManager.ProcessTurnStartDecay();
            }

            if (!activeCharacter.isAlive || activeCharacter.debuffManager.stunTurnsRemaining > 0 || activeCharacter.debuffManager.breakTurnsRemaining > 0)
            {
                EndTurn(activeCharacter);
                yield break;
            }

            if (activeCharacter.isPlayer)
            {
                if (activeCharacter.ownUI != null)
                {
                    foreach (var p in allCombatants.Where(c => c.isPlayer && c.ownUI != null)) p.ownUI.Hide();
                    activeCharacter.ownUI.ShowUI();
                    activeCharacter.ownUI.SetupSkillUI(activeCharacter.skills);
                    activeCharacter.ownUI.SetupSummonUI(activeCharacter.skills);
                    activeCharacter.ownUI.SetActiveCharacter(activeCharacter);
                }
                else EndTurn(activeCharacter);
            }
            else
            {
                yield return StartCoroutine(EnemyTurn(activeCharacter));
            }

            uiManager.UpdateAllCharacterUIs(allCombatants);
        }

        private IEnumerator EnemyTurn(Character enemy)
        {
            yield return new WaitForSeconds(0.5f);

            Enemy enemyComp = enemy.GetComponent<Enemy>();
            enemyComp.PrepareTurn();

            Character playerTarget = enemyComp.target;

            enemyComp.ExecuteTurn();
        }



        public void EndTurn(Character character)
        {
            if (character == activeCharacter || activeCharacter == null)
            {
                CameraAction.instance.TargetAllEnemies();

                foreach (var c in allCombatants)
                {
                    c.isAttackBlocked = false;
                    c.isParrySuccessful = false;
                }

                if (character != null && character.stateMachine != null)
                {
                    character.stateMachine.SwitchState(character.stateMachine.waitingState);
                    character.actionGauge = 0;
                }

                if (!character.isPlayer)
                {
                    EventBus<ShowPanelEvent>.Raise(new ShowPanelEvent("EnemyUI"));
                }

                activeCharacter = null;
                isProcessingTurn = false;
                if (turnHandler != null) turnHandler.isProcessingTurn = false;

                CheckWaveCondition();
            }
        }

        private void CheckWinCondition(bool finalWin = false)
        {
            if (finalWin) TB_Menu.instance.ShowVictoryMenu();
        }

        public void ChooseRandomRule()
        {
            if (availableRules != null && availableRules.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableRules.Count);
                currentRule = availableRules[randomIndex];
            }
        }

        public void ShowCombatantButtonsForFaction(bool showPlayers)
        {
            if (uiManager != null)
            {
                uiManager.SpawnCombatantButtons(showPlayers, allCombatants);
            }
        }

        public Character SummonPet(Character summoner, GameObject petPrefab)
        {
            if (spawner != null)
            {
                return spawner.SummonPet(summoner, petPrefab);
            }
            return null;
        }
        public void RemoveCombatant(Character character)
        {
            if (allCombatants.Contains(character))
            {
                allCombatants.Remove(character);

                if (uiManager != null)
                {
                    uiManager.RemoveCharacterUI(character);
                }

                if (turnOrderUI != null)
                {
                    turnOrderUI.UpdateActionGaugeUI(allCombatants);
                }

                CheckWaveCondition();
            }
        }


        public void TriggerEvadeOnly(float duration, Character player, Enemy enemy)
        {
            if (player.isAttackBlocked) return;

            evadeUI.StartGame(duration, (isSuccess) => {

                if (isSuccess)
                {

                    player.isAttackBlocked = true;
                    enemy.isAttackBlocked = true;

                    if (enemy.stateMachine != null)
                        enemy.stateMachine.SwitchState(new InterruptedState(enemy.stateMachine));

                    if (player.stateMachine != null)
                        player.stateMachine.SwitchState(new AvoidState(player.stateMachine));
                }
            });
        }

        public void TriggerParryOnly(float duration, Character player, Enemy enemy)
        {
            if (player.isAttackBlocked) return;

            parryUI.StartGame(duration, (isSuccess) => {
                if (isSuccess)
                {
                    player.isParrySuccessful = true;
                    player.isAttackBlocked = true;
                    enemy.isAttackBlocked = true;

                    if (enemy.stateMachine != null)
                        enemy.stateMachine.SwitchState(new InterruptedState(enemy.stateMachine));

                    if (player.stateMachine != null)
                        player.stateMachine.SwitchState(new AvoidState(player.stateMachine));
                }
            });
        }


        public Character SpawnCombatant(GameObject prefab, bool isPlayerFaction, Vector3 positionHint)
        {
            if (spawner != null)
            {
                return spawner.SpawnCombatant(prefab, isPlayerFaction, positionHint);
            }
            return null;
        }
    }
}