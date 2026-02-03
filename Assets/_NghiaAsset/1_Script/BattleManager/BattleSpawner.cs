using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Turnbase
{
    public class BattleSpawner : MonoBehaviour
    {
        private BattleManager bm;

        [Header("Enemy Spawn Settings")]
        public FlyweightSettings_TB enemySpawnEffect;
        public float enemyRiseDuration = 0.8f;
        public float enemySinkDepth = 2.5f;

        [Header("Player Spawn Settings")]
        public float playerMoveDuration = 1.0f;
        // Khoảng cách Player sẽ bắt đầu so với vị trí Slot mặc định (Ví dụ: x = -3)
        public Vector3 playerOffsetFromSlot = new Vector3(3f, 0f, 0f);

        public void Initialize(BattleManager manager)
        {
            bm = manager;
        }

        public void SpawnWaveImmediately(int waveIndex, EnemyEncounter encounter, Transform[] enemySlots)
        {
            if (encounter == null || waveIndex >= encounter.waves.Length) return;

            EnemyWave currentWave = encounter.waves[waveIndex];
            Character[] enemiesToSpawn = currentWave.enemiesInWave;

            int enemyCount = Mathf.Min(enemySlots.Length, enemiesToSpawn.Length);
            for (int i = 0; i < enemyCount; i++)
            {
                if (enemiesToSpawn[i] == null) continue;
                SpawnCombatant(enemiesToSpawn[i].gameObject, false, enemySlots[i].position);
            }

            if (bm.turnOrderUI != null) bm.turnOrderUI.UpdateActionGaugeUI(bm.allCombatants);
        }

        public IEnumerator SpawnWaveRoutine(int waveIndex, EnemyEncounter encounter, Transform[] enemySlots)
        {
            if (encounter == null || waveIndex >= encounter.waves.Length) yield break;

            yield return new WaitForSeconds(2f);

            EnemyWave currentWave = encounter.waves[waveIndex];
            Character[] enemiesToSpawn = currentWave.enemiesInWave;

            int enemyCount = Mathf.Min(enemySlots.Length, enemiesToSpawn.Length);
            for (int i = 0; i < enemyCount; i++)
            {
                if (enemiesToSpawn[i] == null) continue;
                SpawnCombatant(enemiesToSpawn[i].gameObject, false, enemySlots[i].position);
            }

            if (bm.turnOrderUI != null) bm.turnOrderUI.UpdateActionGaugeUI(bm.allCombatants);
        }

        public Character SpawnCombatant(GameObject prefab, bool isPlayerFaction, Vector3 positionHint)
        {
            if (prefab == null) return null;

            Transform[] slotArray = isPlayerFaction ? bm.playerSpawnPoints : bm.enemySlots;
            Transform freeSlot = FindFreeSlot(slotArray, positionHint);

            Vector3 finalPosition = freeSlot != null ? freeSlot.position : positionHint;
            Quaternion spawnRotation = freeSlot != null ? freeSlot.rotation : Quaternion.identity;

            GameObject instance = Instantiate(prefab, finalPosition, spawnRotation);
            Character characterInstance = instance.GetComponent<Character>();

            if (characterInstance == null)
            {
                Destroy(instance);
                return null;
            }

            if (freeSlot != null) characterInstance.transform.SetParent(freeSlot);
            characterInstance.initialPosition = finalPosition;
            characterInstance.isPlayer = isPlayerFaction;
            characterInstance.battleManager = bm;
            characterInstance.battleUIManager = bm.uiManager;

            // --- PHÂN CHIA LOGIC XUẤT HIỆN ---
            if (isPlayerFaction)
            {
                // Player: Di chuyển từ vị trí (Slot - Offset) tới vị trí Slot
                Vector3 startPos = finalPosition + playerOffsetFromSlot;
                StartCoroutine(MoveToPosition(characterInstance.transform, startPos, finalPosition, playerMoveDuration));
            }
            else
            {
                // Enemy: Tạo hiệu ứng VFX và trồi lên từ dưới đất ngay tại Slot
                if (enemySpawnEffect != null)
                {
                    Flyweight_TB effect = FlyweightFactory_TB.Spawn(enemySpawnEffect);
                    if (effect != null) effect.Initialize(finalPosition, Quaternion.identity);
                }
                Vector3 startPos = finalPosition + Vector3.down * enemySinkDepth;
                StartCoroutine(MoveToPosition(characterInstance.transform, startPos, finalPosition, enemyRiseDuration));
            }

            // Thiết lập StateMachine, Stats và UI
            SetupCharacter(characterInstance);

            return characterInstance;
        }

        private IEnumerator MoveToPosition(Transform target, Vector3 startPos, Vector3 finalPos, float duration)
        {
            target.position = startPos;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                target.position = Vector3.Lerp(startPos, finalPos, Mathf.SmoothStep(0, 1, t));
                yield return null;
            }

            target.position = finalPos;
        }

        private void SetupCharacter(Character characterInstance)
        {
            CharacterStateMachine stateMachine = characterInstance.GetComponent<CharacterStateMachine>();
            if (stateMachine != null)
            {
                stateMachine.battleManager = bm;
                stateMachine.SwitchState(stateMachine.waitingState);
                characterInstance.actionGauge = 0;
            }

            if (characterInstance.stats != null)
            {
                characterInstance.stats.currentShield = 0;
                characterInstance.stats.currentHP = characterInstance.stats.maxHP;
                characterInstance.stats.currentMP = characterInstance.stats.maxMP;
            }

            PlayerActionUI actionUI = characterInstance.GetComponentInChildren<PlayerActionUI>(true);
            if (actionUI != null)
            {
                actionUI.SetOwner(characterInstance);
                actionUI.Hide();
                characterInstance.ownUI = actionUI;
            }

            characterInstance.isAttackBlocked = false;
            characterInstance.isParrySuccessful = false;
            characterInstance.isParryable = false;

            bm.allCombatants.Add(characterInstance);

            if (bm.uiManager != null) bm.uiManager.SpawnCharacterUI(characterInstance);
            if (bm.turnOrderUI != null) bm.turnOrderUI.UpdateActionGaugeUI(bm.allCombatants);

            bm.turnbuffManager.ProcessOnBattleStartPassives(characterInstance);
        }

        public Transform FindFreeSlot(Transform[] slots, Vector3 positionHint)
        {
            return slots
                .Where(slot => {
                    for (int i = 0; i < slot.childCount; i++)
                    {
                        Character c = slot.GetChild(i).GetComponent<Character>();
                        if (c != null && c.isAlive) return false;
                    }
                    return true;
                })
                .OrderBy(slot => Vector3.Distance(slot.position, positionHint))
                .FirstOrDefault();
        }

        public Character SummonPet(Character summoner, GameObject petPrefab)
        {
            Transform freeSlot = FindFreeSlot(bm.playerSpawnPoints, summoner.transform.position);
            if (freeSlot == null) return null;

            return SpawnCombatant(petPrefab, summoner.isPlayer, freeSlot.position);
        }
    }
}