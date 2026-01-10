using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using UnityEngine.Playables;

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

        public bool isMiniGameRunning = false;

        void Start()
        {
            SetupBattle();
            if (uiManager != null)
            {
                uiManager.InitializeCombatantButtons(allCombatants, statDisplayPanel, this);
            }

            ChooseRandomRule();

        }

        void Update()
        {
            if (activeCharacter != null && !activeCharacter.isPlayer && currentParryWindow != null)
            {
                Enemy enemy = activeCharacter as Enemy;
                Character playerTarget = enemy?.target;

                if (playerTarget != null && playerTarget.isParryable && Input.GetKeyDown(KeyCode.Space))
                {
                    OnParryAttempted();
                }
            }
        }

        public void ChooseRandomRule()
        {
            if (availableRules != null && availableRules.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, availableRules.Count);
                currentRule = availableRules[randomIndex];
                Debug.Log($"[BATTLE START] Luật trận đấu được chọn: {currentRule.ruleName}");
            }
        }



        public void ShowCombatantButtonsForFaction(bool showPlayers)
        {
            if (uiManager != null)
            {
                uiManager.SpawnCombatantButtons(showPlayers, allCombatants);
            }
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
                playerInstance.battleUIManager = this.uiManager;

                if (playerInstance.stats != null)
                {
                    playerInstance.stats.currentShield = 0;
                }

                turnbuffManager.ProcessOnBattleStartPassives(playerInstance);

                if (uiManager != null) uiManager.SpawnCharacterUI(playerInstance);

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

            if (roundTrackerPrefab != null)
            {
                RoundTracker trackerInstance = Instantiate(roundTrackerPrefab, Vector3.zero, Quaternion.identity);
                trackerInstance.battleManager = this;
                trackerInstance.stats.currentHP = trackerInstance.stats.maxHP;

                trackerInstance.isVirtualTracker = true;

                allCombatants.Add(trackerInstance);
                this.instantiatedRoundTracker = trackerInstance;
                Debug.Log("[RoundTracker] đã được thêm vào danh sách chiến đấu.");
            }

            currentWaveIndex = 0;
            if (encounterToLoad != null && encounterToLoad.waves.Length > 0)
            {
                if (uiManager != null)
                {
                    uiManager.UpdateWaveDisplay(currentWaveIndex + 1, encounterToLoad.waves.Length);
                }
                StartCoroutine(SpawnWave(currentWaveIndex));
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

        private IEnumerator SpawnWave(int waveIndex)
        {
            if (encounterToLoad == null || waveIndex >= encounterToLoad.waves.Length)
            {
                CheckWinCondition(true);
                yield break;
            }

            EnemyWave currentWave = encounterToLoad.waves[waveIndex];
            Character[] enemiesToSpawn = currentWave.enemiesInWave;

            Debug.Log($"[WAVE START] Bắt đầu đợt quái số {waveIndex + 1} với {enemiesToSpawn.Length} kẻ địch.");

            if (encounterToLoad != null && uiManager != null)
            {
                uiManager.UpdateWaveDisplay(waveIndex + 1, encounterToLoad.waves.Length);
            }

            yield return new WaitForSeconds(3f);


            int enemyCount = Mathf.Min(enemySlots.Length, enemiesToSpawn.Length);
            for (int i = 0; i < enemyCount; i++)
            {
                Character enemyPrefab = enemiesToSpawn[i];
                if (enemyPrefab == null) continue;

                Character enemyInstance = Instantiate(enemyPrefab, enemySlots[i].position, enemySlots[i].rotation);
                enemyInstance.transform.SetParent(enemySlots[i]);

                enemyInstance.isPlayer = false;
                allCombatants.Add(enemyInstance);
                enemyInstance.initialPosition = enemySlots[i].position;
                enemyInstance.battleManager = this;
                enemyInstance.battleUIManager = this.uiManager;

                if (enemyInstance.stats != null) enemyInstance.stats.currentShield = 0;
                turnbuffManager.ProcessOnBattleStartPassives(enemyInstance);

                CharacterStateMachine enemyStateMachine = enemyInstance.GetComponent<CharacterStateMachine>();
                if (enemyStateMachine != null)
                {
                    enemyStateMachine.battleManager = this;
                }

                if (enemyInstance.stateMachine != null)
                {
                    enemyInstance.stateMachine.SwitchState(enemyInstance.stateMachine.waitingState);
                    enemyInstance.actionGauge = 0;
                }

                if (uiManager != null) uiManager.SpawnCharacterUI(enemyInstance);
            }

            if (turnOrderUI != null)
            {
                turnOrderUI.UpdateActionGaugeUI(allCombatants);
            }

            if (!isActionGaugeRunning)
            {
                isActionGaugeRunning = true;
                StartCoroutine(UpdateActionGauge());
            }
        }

        //win

        private void CheckWinCondition(bool finalWin = false)
        {
            if (finalWin)
            {
                TB_Menu.instance.ShowVictoryMenu();
                return;
            }
        }

        private void CheckWaveCondition()
        {
            var livingEnemiesInCurrentWave = allCombatants
              .Where(c => !c.isPlayer && c.isAlive && !c.isVirtualTracker)
              .ToList();

            if (livingEnemiesInCurrentWave.Count == 0)
            {
                currentWaveIndex++;

                if (encounterToLoad != null && currentWaveIndex < encounterToLoad.waves.Length)
                {
                    allCombatants.RemoveAll(c => c != null && !c.isPlayer && !c.isAlive);

                    if (uiManager != null)
                    {
                        uiManager.UpdateWaveDisplay(currentWaveIndex + 1, encounterToLoad.waves.Length);
                    }

                    StartCoroutine(SpawnWave(currentWaveIndex));
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
                    TB_Menu.instance.ShowLoseMenu();
                }
            }
        }


        private Transform FindFreePlayerSpawnSlot()
        {
            foreach (Transform slot in playerSpawnPoints)
            {
                bool isOccupied = false;
                for (int i = 0; i < slot.childCount; i++)
                {
                    Transform child = slot.GetChild(i);
                    Character character = child.GetComponent<Character>();
                    if (character != null && character.isAlive)
                    {
                        isOccupied = true;
                        break;
                    }
                }

                if (!isOccupied)
                {
                    return slot;
                }
            }
            return null;
        }



        public Character SummonPet(Character summoner, GameObject petPrefab)
        {
            if (petPrefab == null)
            {
                return null;
            }

            Transform freeSlot = FindFreePlayerSpawnSlot();

            if (freeSlot == null)
            {
                return null;
            }

            Vector3 petSpawnPosition = freeSlot.position;
            Quaternion petRotation = freeSlot.rotation;

            GameObject petInstanceObject = Instantiate(petPrefab, petSpawnPosition, petRotation);
            Character summonInstance = petInstanceObject.GetComponent<Character>();

            if (summonInstance != null)
            {
                summonInstance.transform.SetParent(freeSlot);

                summonInstance.isPlayer = summoner.isPlayer;

                summonInstance.battleManager = this;
                summonInstance.initialPosition = petSpawnPosition;

                PlayerActionUI actionUI = summonInstance.GetComponentInChildren<PlayerActionUI>(true);
                if (actionUI != null)
                {
                    actionUI.SetOwner(summonInstance);
                    actionUI.Hide();

                    summonInstance.ownUI = actionUI;
                }

                CharacterStateMachine summonStateMachine = summonInstance.GetComponent<CharacterStateMachine>();
                if (summonStateMachine != null)
                {
                    summonStateMachine.battleManager = this;
                    summonInstance.battleUIManager = this.uiManager;
                    summonStateMachine.SwitchState(summonStateMachine.waitingState);
                }

                allCombatants.Add(summonInstance);

                if (uiManager != null) uiManager.SpawnCharacterUI(summonInstance);

                if (turnOrderUI != null)
                {
                    turnOrderUI.UpdateActionGaugeUI(allCombatants);
                }


                return summonInstance;
            }
            else
            {
                Destroy(petInstanceObject);
                return null;
            }
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
                            StartCoroutine(AdvanceTurn(readyCharacters.First()));
                        }
                    }
                }
                yield return null;
            }
        }

        public IEnumerator AdvanceTurn(Character characterToAct)
        {
            if (activeCharacter != null) yield break;

            activeCharacter = characterToAct;
            Debug.Log($"Đến lượt: {activeCharacter.gameObject.name}");

            if (activeCharacter.isAlive && activeCharacter.stats != null)
            {
                activeCharacter.stats.currentMP = Mathf.Min(activeCharacter.stats.currentMP + 20, activeCharacter.stats.maxMP);
                uiManager.UpdateAllCharacterUIs(allCombatants);
                Debug.Log($"[{activeCharacter.gameObject.name}] bắt đầu lượt và hồi 20 Mana.");


            }

            if (activeCharacter is RoundTracker roundTracker)
            {
                Debug.Log("[RoundTracker] Đến lượt. Thực thi Phase.");

                roundTracker.ExecuteRoundPhase();

                if (currentRule != null)
                {
                    yield return StartCoroutine(currentRule.ExecuteRule(this, activeCharacter));
                    yield return new WaitForSeconds(3f);
                    EndTurn(roundTracker);
                    Debug.Log("BUff Rule");
                }


                if (roundTracker.currentRound <= -1)
                {
                    Debug.Log($"[LOSE CONDITION] Trận đấu kết thúc! Đã đạt đến giới hạn {startRounds} vòng đấu.");

                    TB_Menu.instance.ShowLoseMenu();

                    activeCharacter = null;
                    isProcessingTurn = true;
                    yield break;
                }

                yield break;
            }


            yield return new WaitForSeconds(1f);
            CameraAction.instance.LookCameraAtTarget(activeCharacter);

            if (turnOrderUI != null)
            {
                turnOrderUI.HighlightActiveCharacter(activeCharacter);
            }


            if (!activeCharacter.isAlive)
            {
                Debug.Log($"{activeCharacter.name} đã bị hạ gục bởi Rule!");
                EndTurn(activeCharacter);
                yield break;
            }

            turnbuffManager.ProcessPassiveSkills(activeCharacter);

            if (activeCharacter.buffManager != null)
            {
                activeCharacter.buffManager.ProcessTurnStartDecay();
            }

            if (activeCharacter.debuffManager != null)
            {
                yield return StartCoroutine(activeCharacter.debuffManager.ApplyDoTDamage());
                activeCharacter.debuffManager.ProcessTurnStartDecay();
            }

            if (!activeCharacter.isAlive)
            {
                Debug.Log($"{activeCharacter.name} đã bị hạ gục bởi Debuff DoT!");
                activeCharacter.actionGauge = 0;
                isProcessingTurn = false;
                activeCharacter = null;
                yield break;
            }

            if (activeCharacter.debuffManager.stunTurnsRemaining > 0 || activeCharacter.debuffManager.breakTurnsRemaining > 0)
            {
                activeCharacter.actionGauge = 0;
                if (activeCharacter.stateMachine != null)
                {
                    activeCharacter.stateMachine.SwitchState(activeCharacter.stateMachine.waitingState);
                }

                activeCharacter = null;
                isProcessingTurn = false;
                yield break;
            }


            EventBus<ShowPanelEvent>.Raise(new ShowPanelEvent(panelName: "EnemyUI"));




            if (activeCharacter.stateMachine != null)
            {
                activeCharacter.stateMachine.SwitchState(activeCharacter.stateMachine.waitingState);
            }

            if (activeCharacter.isPlayer)
            {
                if (activeCharacter.ownUI != null)
                {
                    foreach (var player in allCombatants.Where(c => c.isPlayer && c.ownUI != null))
                    {
                        if (player.ownUI != null) player.ownUI.Hide();
                    }

                    activeCharacter.ownUI.ShowUI();
                    activeCharacter.ownUI.SetupSkillUI(activeCharacter.skills);
                    activeCharacter.ownUI.SetupSummonUI(activeCharacter.skills);
                    activeCharacter.ownUI.SetActiveCharacter(activeCharacter);
                }
                else
                {
                    EndTurn(activeCharacter);
                }
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

            if (playerTarget != null && playerTarget.isPlayer)
            {
                bool miniGameFinished = false;
                playerTarget.isParrySuccessful = false;

                parryUI.StartGame(2.5f, (isSuccess) => {
                    if (isSuccess)
                    {
                        playerTarget.isAttackBlocked = true;
                        enemyComp.isAttackBlocked = true;

                        if (playerTarget.animator != null)
                        {
                            playerTarget.animator.Play("Standing");
                        }

                        Debug.Log("<color=green>[SYSTEM]</color> Mini-game thành công, đã KHÓA sát thương.");
                        OnParryAttempted();
                    }
                    else
                    {
                        playerTarget.isAttackBlocked = false;
                        enemyComp.isAttackBlocked = false;

                        Debug.Log("<color=red>[SYSTEM]</color> Mini-game thất bại! Người chơi sẽ nhận sát thương.");
                    }
                    miniGameFinished = true;
                });

                yield return new WaitUntil(() => miniGameFinished);
                yield return new WaitForSeconds(0.5f);
            }

            enemyComp.ExecuteTurn();
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
                if (target != null)
                {
                    enemy.isAttackBlocked = true;
                    target.isAttackBlocked = true;
                    target.isParrySuccessful = true;

                    Debug.Log($"<color=green>[SYSTEM]</color> Đã set isAttackBlocked = true cho {target.name}");

                    int parryDamage = Mathf.RoundToInt(target.stats.physicalAttack * 1.5f);
                    enemy.TakeDamage(parryDamage, ElementType.None);

                }
            }
        }


        public Character SpawnCombatant(GameObject prefab, bool isPlayerFaction, Vector3 positionHint)
        {
            if (prefab == null)
            {
                Debug.LogError("[SpawnCombatant] Prefab rỗng!");
                return null;
            }

            Transform[] slotArray = isPlayerFaction ? playerSpawnPoints : enemySlots;

            Transform freeSlot = null;
            Vector3 spawnPosition;
            Quaternion spawnRotation;

            if (isPlayerFaction)
            {
                freeSlot = FindFreePlayerSpawnSlot();
            }
            else 
            {
                if (slotArray.Length > 0)
                {
                    var occupiedEnemySlots = new HashSet<Transform>();
                    foreach (Transform slot in enemySlots)
                    {
                        for (int i = 0; i < slot.childCount; i++)
                        {
                            Character character = slot.GetChild(i).GetComponent<Character>();
                            if (character != null && character.isAlive)
                            {
                                occupiedEnemySlots.Add(slot);
                                break;
                            }
                        }
                    }

                    var availableEnemySlots = slotArray.Where(slot => !occupiedEnemySlots.Contains(slot)).ToList();

                    if (availableEnemySlots.Count > 0)
                    {
                        freeSlot = availableEnemySlots
            .OrderBy(slot => Vector3.Distance(slot.position, positionHint))
            .FirstOrDefault();
                    }
                }
            }

            if (freeSlot != null)
            {
                spawnPosition = freeSlot.position;
                spawnRotation = freeSlot.rotation;
            }
            else
            {
                if (!isPlayerFaction)
                {
                    Debug.LogWarning($"[SpawnCombatant] Hủy spawn đơn vị địch {prefab.name}. Không còn slot trống.");
                    return null;
                }

                spawnPosition = positionHint;
                spawnRotation = Quaternion.identity;
                Debug.LogWarning($"[SpawnCombatant] Không tìm được slot trống cho Player. Spawn tại vị trí gợi ý.");
            }

            GameObject instance = Instantiate(prefab, spawnPosition, spawnRotation);
            Character characterInstance = instance.GetComponent<Character>();

            if (characterInstance != null)
            {
                if (freeSlot != null)
                {
                    characterInstance.transform.SetParent(freeSlot);
                    characterInstance.initialPosition = freeSlot.position;
                }
                else
                {
                    characterInstance.initialPosition = positionHint;
                }

                characterInstance.isPlayer = isPlayerFaction;

                characterInstance.battleManager = this;

                if (this.uiManager != null)
                {
                    characterInstance.battleUIManager = this.uiManager;
                }

                CharacterStateMachine characterStateMachine = characterInstance.GetComponent<CharacterStateMachine>();
                if (characterStateMachine != null)
                {
                    characterStateMachine.battleManager = this;
                    characterStateMachine.SwitchState(characterStateMachine.waitingState);
                    characterInstance.actionGauge = 0;
                }
                if (uiManager != null) uiManager.SpawnCharacterUI(characterInstance);


                allCombatants.Add(characterInstance);

                if (characterInstance.stats != null)
                {
                    characterInstance.stats.currentHP = characterInstance.stats.maxHP;
                    characterInstance.stats.currentMP = characterInstance.stats.maxMP;
                    characterInstance.stats.currentShield = 0;
                }

                if (turnOrderUI != null)
                {
                    turnOrderUI.UpdateActionGaugeUI(allCombatants);
                }

                turnbuffManager.ProcessOnBattleStartPassives(characterInstance);

                Debug.Log($"Đã spawn đơn vị mới: {characterInstance.name}. Faction: {(isPlayerFaction ? "Player" : "Enemy")}. Slot: {(freeSlot != null ? freeSlot.name : "None")}");
            }
            else
            {
                Destroy(instance);
                Debug.LogError($"Prefab {prefab.name} không có component Character!");
                return null;
            }

            return characterInstance;
        }
        public void EndTurn(Character character)
        {
            if (character == activeCharacter)
            {
                EventBus<ShowPanelEvent>.Raise(new ShowPanelEvent(panelName: "EnemyUI"));

                CameraAction.instance.TargetAllEnemies();

                foreach (var c in allCombatants)
                {
                    c.isAttackBlocked = false;
                    c.isParrySuccessful = false; 
                }

                activeCharacter = null;
                if (character.stateMachine != null)
                {
                    character.stateMachine.SwitchState(character.stateMachine.waitingState);
                    character.actionGauge = 0;
                }
                isProcessingTurn = false;
            }
        }

        //private IEnumerator LoadMapSceneDelayed(string sceneName, float delay)
        //{
        //    yield return new WaitForSeconds(delay);

        //    FlyweightFactory_TB.Instance.ClearAllPools();

        //    SceneManager.LoadScene(sceneName);
        //}
    }
}