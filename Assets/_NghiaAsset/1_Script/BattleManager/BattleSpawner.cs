using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MyRule;
using MyRule.Audio;
using Cysharp.Threading.Tasks;

namespace Turnbase
{
    public class BattleSpawner : MonoBehaviour
    {
        private BattleManager bm;

        [SerializeField] private TopdownWarpDriveController _warpDriveController;

        [Header("Enemy Spawn Settings")]
        public FlyweightSettings_TB enemySpawnEffect;
        public float enemyRiseDuration = 0.8f;
        public float enemySinkDepth = 2.5f;

        [Header("Player Spawn Settings")]
        public float playerMoveDuration = 0.3f;
        public Vector3 playerOffsetFromSlot = new Vector3(3f, 0f, 0f);

        public void Initialize(BattleManager manager)
        {
            bm = manager;
        }

        public void SpawnWaveImmediately(WaveData waveData, Transform[] enemySlots)
        {
            if (waveData == null) return;

            OnSoundMonster();

            EnemyData[] enemiesToSpawn = waveData.Enemies;
            bool bossSpawnedInThisWave = false;

            int enemyCount = Mathf.Min(enemySlots.Length, enemiesToSpawn.Length);
            for (int i = 0; i < enemyCount; i++)
            {
                if (enemiesToSpawn[i] == null) continue;

                EnemyDataSO so = WaveManager.Instance.GetEnemySOById(enemiesToSpawn[i].EnemyId);
                if (so == null || so.enemyPrefab == null) continue;

                // Kiểm tra điều kiện Boss
                Enemy enemyComp = so.enemyPrefab.GetComponent<Enemy>();
                bool isBossPrefab = (enemyComp != null && enemyComp.isBoss);

                if (isBossPrefab && bossSpawnedInThisWave) continue;

                Character character = SpawnCombatant(so.enemyPrefab.gameObject, false, enemySlots[i].position);

                if (character != null)
                {
                    ApplyScaledStats(character, enemiesToSpawn[i]);
                    if (isBossPrefab) bossSpawnedInThisWave = true;
                }
            }

            if (bm.turnOrderUI != null) bm.turnOrderUI.UpdateActionGaugeUI(bm.allCombatants);
        }

        private void OnSoundMonster()
        {
            AudioManager.Instance.PlaySFX(SFXType.EnemySound);
        }

        public IEnumerator SpawnWaveRoutine(WaveData waveData, Transform[] enemySlots)
        {
            if (waveData == null) yield break;

            yield return new WaitForSeconds(2f);

            EnemyData[] enemiesToSpawn = waveData.Enemies;
            bool bossSpawnedInThisWave = false;

            int enemyCount = Mathf.Min(enemySlots.Length, enemiesToSpawn.Length);
            for (int i = 0; i < enemyCount; i++)
            {
                if (enemiesToSpawn[i] == null) continue;

                EnemyDataSO so = WaveManager.Instance.GetEnemySOById(enemiesToSpawn[i].EnemyId);
                if (so == null || so.enemyPrefab == null) continue;

                Enemy enemyComp = so.enemyPrefab.GetComponent<Enemy>();
                bool isBossPrefab = (enemyComp != null && enemyComp.isBoss);

                if (isBossPrefab && bossSpawnedInThisWave) continue;

                Character character = SpawnCombatant(so.enemyPrefab.gameObject, false, enemySlots[i].position);

                if (character != null)
                {
                    ApplyScaledStats(character, enemiesToSpawn[i]);
                    if (isBossPrefab) bossSpawnedInThisWave = true;
                }
            }

            if (bm.turnOrderUI != null) bm.turnOrderUI.UpdateActionGaugeUI(bm.allCombatants);
        }

        private void ApplyScaledStats(Character character, EnemyData data)
        {
            if (character.stats == null) return;

            CharacterStats s = character.stats;
            s.maxHP = data.Health;
            s.currentHP = data.Health;
            s.physicalAttack = data.Phys;
            s.magicAttack = data.Mag;
            s.physicalDefense = data.PhyDef;
            s.magicDefense = data.MagDef;
            s.fireDefense = data.FireDef;
            s.frostDefense = data.FrostDef;
            s.lightningDefense = data.LightningDef;
            s.holyDefense = data.HolyDef;
            s.darkDefense = data.DarkDef;
            s.waterDefense = data.WaterDef;
            s.poisonDefense = data.PoisonDef;
            s.fireDamageBonus = data.Fire;
            s.frostDamageBonus = data.Frost;
            s.lightningDamageBonus = data.Lightning;
            s.holyDamageBonus = data.Holy;
            s.darkDamageBonus = data.Dark;
            s.waterDamageBonus = data.Water;
            s.poisonDamageBonus = data.Poison;
            s.speed = (int)data.AttackSpeed;
            s.critChance = (int)data.CritChance;
            s.critMult = (int)(data.CritMult * 100);
        }

        public Character SpawnCombatant(GameObject prefab, bool isPlayerFaction, Vector3 positionHint)
        {
            if (prefab == null) return null;

            Enemy eComp = prefab.GetComponent<Enemy>();
            bool isBoss = (eComp != null && eComp.isBoss);

            Transform[] slotArray = isPlayerFaction ? bm.playerSpawnPoints : bm.enemySlots;
            Transform freeSlot = FindFreeSlot(slotArray, positionHint, isBoss);

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

            if (isPlayerFaction)
            {
                Vector3 startPos = finalPosition + playerOffsetFromSlot;
                StartCoroutine(MoveWithDelay(characterInstance.transform, startPos, finalPosition, playerMoveDuration, 5f));
            }
            else
            {
                if (enemySpawnEffect != null)
                {
                    Flyweight_TB effect = FlyweightFactory_TB.Spawn(enemySpawnEffect);
                    if (effect != null) effect.Initialize(finalPosition, Quaternion.identity);
                }
                Vector3 startPos = finalPosition + Vector3.down * enemySinkDepth;
                StartCoroutine(MoveToPosition(characterInstance.transform, startPos, finalPosition, enemyRiseDuration));
            }

            SetupCharacter(characterInstance);
            return characterInstance;
        }

        private IEnumerator MoveWithDelay(Transform target, Vector3 startPos, Vector3 finalPos, float duration, float delay)
        {
            if (target == null) yield break;

            target.position = startPos;

            if (_warpDriveController != null)
            {
                _warpDriveController.gameObject.SetActive(false);
                _warpDriveController.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(delay);

            if (target != null)
            {
                yield return StartCoroutine(MoveToPosition(target, startPos, finalPos, duration));
            }
        }


        public async void WarpDriveBackk(Vector3 targetPosition)
        {
            if (_warpDriveController != null)
            {
                _warpDriveController.transform.position = targetPosition;

                _warpDriveController.gameObject.SetActive(false);
                _warpDriveController.gameObject.SetActive(true);

                await UniTask.Delay(2000);

                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                foreach (GameObject p in players)
                {
                    if (p != null)
                    {
                        p.SetActive(false);
                    }
                }

            }
        }

        private IEnumerator MoveToPosition(Transform target, Vector3 startPos, Vector3 finalPos, float duration)
        {
            Animator anim = target.GetComponentInChildren<Animator>();
            if (anim != null) anim.Play("walk");

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
            if (anim != null) anim.Play("Idle");
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

        public Transform FindFreeSlot(Transform[] slots, Vector3 positionHint, bool isBoss = false)
        {
            if (isBoss && slots.Length >= 3)
            {
                Transform slot3 = slots[1];
                bool isOccupied = false;
                for (int i = 0; i < slot3.childCount; i++)
                {
                    Character c = slot3.GetChild(i).GetComponent<Character>();
                    if (c != null && c.isAlive) { isOccupied = true; break; }
                }
                if (!isOccupied) return slot3;
            }

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
            Transform freeSlot = FindFreeSlot(bm.playerSpawnPoints, summoner.transform.position, false);
            if (freeSlot == null) return null;

            return SpawnCombatant(petPrefab, summoner.isPlayer, freeSlot.position);
        }
    }
}