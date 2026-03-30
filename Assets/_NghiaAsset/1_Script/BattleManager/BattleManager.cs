using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using MyRule;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using System.Threading.Tasks;

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
        public GameObject testEnemyPrefab;

        public TurnOrderUI turnOrderUI;

        public BattleUIManager uiManager;

        public ElementChart elementChart;

        private Coroutine currentParryWindow;

        public CharacterStatUI statDisplayPanel;

        public BattleBuffManager turnbuffManager;

        private int currentWaveIndex = 0;
        private GroupWave currentGroupWave;

        private bool isActionGaugeRunning = false;

        [Header("Battle Rules")]
        public List<BattleRule> availableRules;
        private BattleRule currentRule = null;

        [Header("Round Tracking")]
        public RoundTracker roundTrackerPrefab;
        public int roundSpeed = 10;

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

        [Header("VFX Settings")]
        public GameObject parryEffectPrefab;
        public float effectDuration = 2f;

        async void Start()
        {
            isProcessingTurn = true;
            if (turnHandler != null) turnHandler.isProcessingTurn = true;

            if (spawner != null) spawner.Initialize(this);
            if (turnHandler != null) turnHandler.Initialize(this);

            SetupBattle();

            if (uiManager != null)
                uiManager.InitializeCombatantButtons(allCombatants, statDisplayPanel, this);

            ChooseRandomRule();

            if (BattleCutsceneManager.Instance != null)
            {
                await BattleCutsceneManager.Instance.PlayCutscene(BattleCutsceneType.Start);
            }

            isProcessingTurn = false;
            if (turnHandler != null)
            {
                turnHandler.isProcessingTurn = false;
                StartCoroutine(turnHandler.UpdateActionGaugeRoutine());
            }
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
                trackerInstance.currentRound = startRounds;
                trackerInstance.stats.currentHP = trackerInstance.stats.maxHP;
                trackerInstance.isVirtualTracker = true;
                allCombatants.Add(trackerInstance);
                this.instantiatedRoundTracker = trackerInstance;
            }

            currentWaveIndex = 0;

            if (testEnemyPrefab != null)
            {
                if (enemySlots != null && enemySlots.Length > 0)
                {
                    spawner.SpawnCombatant(testEnemyPrefab, false, enemySlots[0].position);
                }
                if (uiManager != null)
                    uiManager.UpdateWaveDisplay(1, 1);
            }
            else
            {
                currentGroupWave = WaveManager.Instance.GetCurrentWave();
                if (currentGroupWave != null && currentGroupWave.WaveDatas.Length > 0)
                {
                    if (uiManager != null)
                        uiManager.UpdateWaveDisplay(currentWaveIndex + 1, currentGroupWave.WaveDatas.Length);

                    WaveData firstWave = currentGroupWave.WaveDatas[currentWaveIndex];
                    spawner.SpawnWaveImmediately(firstWave, enemySlots);
                }
            }

            if (turnOrderUI != null) turnOrderUI.UpdateActionGaugeUI(allCombatants);
        }

        public async void CheckWaveCondition()
        {
            var livingEnemies = allCombatants.Where(c => !c.isPlayer && c.isAlive && !c.isVirtualTracker).ToList();

            if (livingEnemies.Count == 0)
            {
                currentWaveIndex++;

                if (currentGroupWave != null && currentWaveIndex < currentGroupWave.WaveDatas.Length)
                {
                    allCombatants.RemoveAll(c => c != null && !c.isPlayer && !c.isAlive);

                    if (uiManager != null)
                        uiManager.UpdateWaveDisplay(currentWaveIndex + 1, currentGroupWave.WaveDatas.Length);

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
                if (livingPlayers.Count == 0)
                {
                    isProcessingTurn = true;
                    if (turnHandler != null) turnHandler.isProcessingTurn = true;

                    CameraAction.instance.TargetDeadCamera();

                    await UniTask.Delay(3000);
                    //CharacterManager.Instance.SetCurrentHealth(activeCharacter.stats.currentHP);
                    TB_Menu.instance.ShowLoseMenu();
                }
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

            if (currentGroupWave != null && currentWaveIndex < currentGroupWave.WaveDatas.Length)
            {
                WaveData nextWaveData = currentGroupWave.WaveDatas[currentWaveIndex];

                yield return StartCoroutine(spawner.SpawnWaveRoutine(nextWaveData, enemySlots));
            }

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
                EndTurn(roundTracker);
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
            CameraAction.instance.LookCameraAtTarget(activeCharacter);
            if (turnOrderUI != null) turnOrderUI.HighlightActiveCharacter(activeCharacter);

            if (!activeCharacter.isAlive) { EndTurn(activeCharacter); yield break; }

            CharacterIItemBuffManager itemBuffs = activeCharacter.GetComponent<CharacterIItemBuffManager>();
            if (itemBuffs != null)
            {
                itemBuffs.ProcessTurnDecay();
            }

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
                Skill passiveSkill = activeCharacter.skills.FirstOrDefault(s => s.skillType == SkillType.XPassive);

                if (passiveSkill != null)
                {
                    Debug.Log($"<color=cyan>[PASSIVE]</color> Kích hoạt tự động: {passiveSkill.skillName}");
                    ICommand passiveCmd = new XPassiveCommand(activeCharacter, passiveSkill, this);

                    yield return StartCoroutine(passiveCmd.Execute());
                }

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
                    if (c == null) continue;
                    c.isAttackBlocked = false;
                    c.isParrySuccessful = false;
                    c.isLastHit = false;
                    c.parryMissCount = 0;
                    c.currentHitInSequence = 0;
                    c.totalHitsInSequence = 0;
                }

                if (character != null && character.stateMachine != null)
                {
                    character.stateMachine.SwitchState(character.stateMachine.waitingState);

                    character.actionGauge -= 10000f;
                    if (character.actionGauge < 0) character.actionGauge = 0;
                }

                if (character != null && !character.isPlayer)
                {
                    EventBus<ShowPanelEvent>.Raise(new ShowPanelEvent("EnemyUI"));
                }

                activeCharacter = null;
                isProcessingTurn = false;
                if (turnHandler != null) turnHandler.isProcessingTurn = false;

                if (turnOrderUI != null)
                {
                    turnOrderUI.UpdateActionGaugeUI(allCombatants);
                }

                CheckWaveCondition();
            }
        }

        private void CheckWinCondition(bool finalWin = false)
        {
            if (finalWin)
            {
                var mainPlayer = allCombatants.FirstOrDefault(c => c.isPlayer);
                if (mainPlayer != null)
                {
                    CharacterManager.Instance.SetCurrentHealth(mainPlayer.stats.currentHP);
                }
                TB_Menu.instance.ShowVictoryMenu();
            }
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

                if (activeCharacter == character)
                {
                    activeCharacter = null;
                    isProcessingTurn = false;
                    if (turnHandler != null) turnHandler.isProcessingTurn = false;
                }

                CheckWaveCondition();
            }
        }


        public void TriggerEvadeOnly(Character player, Enemy enemy)
        {
            evadeUI.StartGame(1.0f, (isSuccess) => {

                if (isSuccess)
                {

                    player.isAttackBlocked = true;
                    //enemy.isAttackBlocked = true;

                    if (enemy.stateMachine != null)

                    if (player.stateMachine != null)
                        player.stateMachine.SwitchState(new AvoidState(player.stateMachine));
                }
            });
        }

        public void TriggerParryOnly(Character player, Enemy enemy)
        {
            if (player.isParrySuccessful) return; 

            parryUI.onAttempt = () => {
                player.animator.Play("Parry");
                SpawnEffectParry(player);
            };

            parryUI.StartGame(2f, (isSuccess) => {
                if (isSuccess)
                {
                    player.isParrySuccessful = true;
                    player.isAttackBlocked = true;
                    enemy.isAttackBlocked = true;

                    if (enemy != null && enemy.buffEffectSpawnPoint != null && parryEffectPrefab != null)
                    {
                        GameObject effect = Instantiate(parryEffectPrefab,
                                                        enemy.buffEffectSpawnPoint.position,
                                                        enemy.buffEffectSpawnPoint.rotation);

                        effect.transform.SetParent(enemy.buffEffectSpawnPoint);

                        Destroy(effect, 2.0f);
                    }


                    if (player.stateMachine != null)
                        player.stateMachine.SwitchState(new ParryingState(player.stateMachine, enemy));
                }
                else
                {
                    enemy.parryMissCount++;
                }

                parryUI.onAttempt = null;
            });
        }

        public void SpawnEffectParry(Character targetCharacter)
        {
            OneShotVFXSettings_TB settings = Resources.Load<OneShotVFXSettings_TB>("Projectiles/Parry");

            if (settings != null)
            {
                Flyweight_TB effect = FlyweightFactory_TB.Spawn(settings);

                if (effect != null)
                {
                    effect.transform.SetParent(targetCharacter.transform); // Sửa 'character' thành 'targetCharacter'
                    effect.transform.localPosition = Vector3.zero;
                    effect.transform.localRotation = Quaternion.identity;

                    effect.Initialize(targetCharacter.transform.position, targetCharacter.transform.rotation);
                }
            }
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