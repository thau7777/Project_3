using Ami.BroAudio;
using Cysharp.Threading.Tasks;
using MyRule;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TopDownEnemyManager : Singleton<TopDownEnemyManager>
{

    private bool _isBossFight = false;
    [SerializeField]
    private bool _spawnWithLimitCount = false;
    [SerializeField]
    private int _spawnCountLimit = 0;


    [System.Serializable]
    public struct TopdownEnemyWithIds
    {
        public EnemyId id;
        public EnemyTopDownSettings settings;
    }
    [TabGroup("References")]
    [SerializeField] private TextMeshProUGUI _currentWaveText;
    [TabGroup("References")]
    [SerializeField] private TextMeshProUGUI _remainingEnemiesText;
    [TabGroup("References")]
    [SerializeField] private TextMeshProUGUI waveTimeRemainingText;
    [SerializeField, TabGroup("References")]
    private TopDownEnemyUIController _bossStatusBar;

    [TabGroup("Enemy Registry")]
    [SerializeField] private List<TopdownEnemyWithIds> _topDownEnemyWithIdsList;

    [TabGroup("Spawn Settings")]
    [MinMaxSlider(5, 10)]
    [SerializeField] private Vector2 _spawnRadiusMinMax = new Vector2(5, 10);
    [TabGroup("Spawn Settings")]
    [Range(20, 60)]
    [SerializeField] private float _waveDuration = 60f; // 1 minute per wave
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private LayerMask groundLayerMask;

    [TabGroup("Sounds"), SerializeField] private SoundID _enemyDieSound;
    public Transform BossTransform { get; private set; }
    // Wave state
    private GroupWave _groupWave;
    private int _currentWaveIndex = 0;
    public int CurrentWaveIndex
    {
        get { return _currentWaveIndex; }
        set
        {
            _currentWaveIndex = value;
            if (!_isBossFight && _currentWaveText != null)
                _currentWaveText.text = (_currentWaveIndex + 1).ToString() + "/" + _groupWave.WaveDatas.Length;
        }
    }
    private int _remainingEnemiesInWave = 0;
    public int RemainingEnemiesInWave
    {
        get { return _remainingEnemiesInWave; }
        set
        {
            _remainingEnemiesInWave = value;
            if (!_isBossFight && _remainingEnemiesText != null)
                _remainingEnemiesText.text = _remainingEnemiesInWave.ToString();
        }
    }
    private bool _isSpawning = false;
    private bool _gameOver = false;
    private Coroutine _waveTimerCoroutine;
    private float _waveTimeRemaining = 0f;
    public float WaveTimeRemaining
    {
        get { return _waveTimeRemaining; }
        set
        {
            _waveTimeRemaining = value;
            if (!_isBossFight && waveTimeRemainingText != null)
                waveTimeRemainingText.text = Mathf.RoundToInt(_waveTimeRemaining).ToString();
        }
    }

    // Tracked live enemies
    private List<GameObject> _activeEnemies = new List<GameObject>();

    private Transform _player;

    // Events
    private EventBinding<TopdownStartGameEvent> _startGameEventBinding;



    private void OnEnable()
    {
        _startGameEventBinding = new EventBinding<TopdownStartGameEvent>(LoadCurrentWave);
        _startGameEventBinding.Add(SpawnEnemies);
        _startGameEventBinding.Add(StartShowEnemyTutorial);
        EventBus<TopdownStartGameEvent>.Register(_startGameEventBinding);
    }

    private void OnDisable()
    {
        EventBus<TopdownStartGameEvent>.Deregister(_startGameEventBinding);
    }
    private void StartShowEnemyTutorial()
    {
        if (!transform.GetChild(0).gameObject.activeSelf)
            return;
        ShowEnemyTutorial();
    }
    private async UniTaskVoid ShowEnemyTutorial()
    {
        await UniTask.Delay(2000);

        // Find nearest enemy
        GameObject nearestEnemy = _activeEnemies[0];
        foreach (var enemy in _activeEnemies)
        {
            if (Vector3.Distance(_player.position, enemy.transform.position) <
                Vector3.Distance(_player.position, nearestEnemy.transform.position))
            {
                nearestEnemy = enemy;
            }
        }
        // Grab UI elements from the enemy's canvas
        Canvas enemyCanvas = nearestEnemy.GetComponentInChildren<Canvas>();
        GameObject elementUI = enemyCanvas.transform.GetChild(1).gameObject;
        GameObject healthBar = enemyCanvas.transform.GetChild(2).GetChild(0).gameObject;
        GameObject staminaBar = enemyCanvas.transform.GetChild(2).GetChild(1).gameObject;

        // Build 3 step bindings — one per tutorial step
        TutorialStepBinding[] bindings = new TutorialStepBinding[3];

        bindings[0] = new TutorialStepBinding
        {
            highlightTargets = new GameObject[] { healthBar },
            anchorOverride = healthBar.transform
        };

        bindings[1] = new TutorialStepBinding
        {
            highlightTargets = new GameObject[] { staminaBar },
            anchorOverride = staminaBar.transform
        };

        bindings[2] = new TutorialStepBinding
        {
            highlightTargets = new GameObject[] { elementUI },
            anchorOverride = elementUI.transform
        };

        // Setup and fire the tutorial trigger
        TutorialTrigger trigger = transform.GetChild(0).GetComponent<TutorialTrigger>();
        trigger.ManualTriggerSetup(enemyCanvas.transform, bindings);
        trigger.Trigger();
    }
    private void LoadCurrentWave()
    {
        _isBossFight = TopDownGameManager.Instance.isBossFighting;
        if (TopDownGameManager.Instance.isTestGameplay)
        {
            if(_isBossFight)
                _groupWave = WaveManager.Instance.CreateBossWave(true);
            else
                _groupWave = WaveManager.Instance.CreateNewWave(true);

        }
        else
            _groupWave = WaveManager.Instance.GetCurrentWave();

        if (_groupWave == null)
        {
            Debug.LogError("TopDownEnemyManager: No GroupWave returned from WaveManager!");
        }
    }

    private bool IsLastWave() => CurrentWaveIndex >= _groupWave.WaveDatas.Length - 1;

    // Called when the game starts via event
    private void SpawnEnemies()
    {
        _player = PlayerTopDownStateDriver.Instance.transform;
        if (_player == null)
        {
            Debug.LogError("EnemyManager: Player not found! Make sure your Player has the 'Player' tag.");
        }
        if (_groupWave == null)
        {
            Debug.LogError("TopDownEnemyManager: GroupWave is null, cannot spawn enemies.");
            return;
        }

        CurrentWaveIndex = 0;
        _gameOver = false;
        if (_isBossFight)
            StartCoroutine(StartBossFight());
        else
            StartCoroutine(StartWave(CurrentWaveIndex));
    }

    // ─── Boss Fight ───────────────────────────────────────────────────────────

    private IEnumerator StartBossFight()
    {
        WaveData[] waves = _groupWave.WaveDatas;
        if (waves == null || waves.Length == 0 || waves[0] == null)
        {
            Debug.LogError("[BossFight] No valid WaveData found for boss fight.");
            yield break;
        }

        WaveData bossWave = waves[0];
        if (bossWave.Enemies == null || bossWave.Enemies.Length == 0 || bossWave.Enemies[0] == null)
        {
            Debug.LogError("[BossFight] No valid enemy found in wave 0 for boss fight.");
            yield break;
        }


        RemainingEnemiesInWave = 1;
        _isSpawning = true;
        SpawnEnemy(bossWave.Enemies[0]);
        _isSpawning = false;
        CinemachineCameraController.Instance.ShakeSequence(2);

        // No timer — waiting for the boss to die via OnEnemyDied()
    }

    // ─── Wave Lifecycle ───────────────────────────────────────────────────────

    private IEnumerator StartWave(int waveIndex)
    {
        WaveData[] waves = _groupWave.WaveDatas;
        if (waveIndex >= (!_spawnWithLimitCount ? waves.Length : 1)) yield break;

        WaveData wave = waves[waveIndex];
        if (wave == null)
        {
            Debug.LogWarning($"Wave {waveIndex} is null — skipping.");
            yield return StartCoroutine(AdvanceToNextWave());
            yield break;
        }


        // Never reset — always add new wave's enemies on top of surviving stragglers
        int newEnemyCount = 0;
        foreach (var enemy in wave.Enemies)
            if (enemy != null) newEnemyCount++;

        RemainingEnemiesInWave += newEnemyCount;

        _isSpawning = true;

        for (int i = 0; i < (!_spawnWithLimitCount ? wave.Enemies.Length : _spawnCountLimit); i++)
        {
            EnemyData enemyData = wave.Enemies[i];
            if (enemyData == null) continue;

            SpawnEnemy(enemyData);
            //yield return new WaitForSeconds(Random.Range(0, 0.3f));
        }

        _isSpawning = false;

        if (_waveTimerCoroutine != null)
            StopCoroutine(_waveTimerCoroutine);

        _waveTimerCoroutine = StartCoroutine(WaveTimer());
    }

    public void OnEnemyDied(GameObject enemyGO)
    {
        BroAudio.Play(_enemyDieSound);
        _activeEnemies.Remove(enemyGO);

        RemainingEnemiesInWave--;
        RemainingEnemiesInWave = Mathf.Max(0, RemainingEnemiesInWave);

        Debug.Log($"[WaveManager] Enemy died. Remaining: {RemainingEnemiesInWave} | Active tracked: {_activeEnemies.Count}");

        // Boss fight: win immediately when the boss is dead
        if (_isBossFight)
        {
            if (_activeEnemies.Count == 0)
            {
                OnBossDied();
            }
            return;
        }

        // Use _activeEnemies.Count as the authoritative source — catches stragglers too
        if (_activeEnemies.Count == 0)
        {
            if (_waveTimerCoroutine != null)
            {
                StopCoroutine(_waveTimerCoroutine);
                _waveTimerCoroutine = null;
            }

            if (IsLastWave())
            {
                Debug.Log("[WaveManager] Final wave — all enemies killed. YOU WIN!");
                WinGame();
                return;
            }

            Debug.Log($"[WaveManager] Wave {CurrentWaveIndex + 1} cleared early! Advancing.");
            StartCoroutine(AdvanceToNextWave());
        }
    }

    private void OnBossDied()
    {
        BossCameraController.Instance.FocusOnBoss();
        WinGame();

    }
    private IEnumerator WaveTimer()
    {
        WaveTimeRemaining = _waveDuration;

        while (WaveTimeRemaining > 0f)
        {
            yield return null; // tick every frame for smooth UI updates
            WaveTimeRemaining -= Time.deltaTime;
        }

        WaveTimeRemaining = 0f;

        if (_gameOver) yield break;

        // On the final wave, timer expiry does NOT trigger a win — enemies must all be killed
        if (IsLastWave())
        {
            Debug.Log($"[WaveManager] Final wave timer expired — enemies must be killed to win!");
            yield break;
        }

        // Timer expired on a non-final wave — leave enemies alive and advance
        Debug.Log($"[WaveManager] Wave {CurrentWaveIndex + 1} time expired! Moving to next wave.");
        yield return StartCoroutine(AdvanceToNextWave());
    }

    private IEnumerator AdvanceToNextWave()
    {
        if (_gameOver) yield break;

        WaveTimeRemaining = 0f;

        yield return new WaitForSeconds(2f); // brief pause between waves

        CurrentWaveIndex++;
        yield return StartCoroutine(StartWave(CurrentWaveIndex));
    }

    // ─── Enemy Spawning ───────────────────────────────────────────────────────

    private void SpawnEnemy(EnemyData enemyData)
    {
        EnemyTopDownSettings settings = GetSettingsForId(enemyData.EnemyId);
        if (settings == null)
        {
            Debug.LogWarning($"No settings found for EnemyId: {enemyData.EnemyId}");
            RemainingEnemiesInWave--;
            return;
        }
        settings.SetupSpawnSettings(PlayerTopDownStateDriver.Instance.transform,_isBossFight ? _spawnRadiusMinMax.y : Random.Range(_spawnRadiusMinMax.x, _spawnRadiusMinMax.y), groundLayerMask, obstacleLayerMask);

        var enemy = FlyweightFactory.Spawn(settings);
        if (enemy.TryGetComponent<CharacterStats>(out var enemyStats))
        {
            enemyStats.Setup(settings.elementalType, enemyData.Health, enemyData.Stamina, 0, enemyData.Phys, enemyData.Mag, enemyData.Fire, enemyData.Water, enemyData.Frost,
                enemyData.Lightning, enemyData.Holy, enemyData.Dark, enemyData.Poison, enemyData.PhyDef, enemyData.MagDef, enemyData.FireDef, enemyData.WaterDef,
                enemyData.FrostDef, enemyData.LightningDef, enemyData.HolyDef, enemyData.DarkDef, enemyData.PoisonDef, enemyData.AttackSpeed, enemyData.CritChance, enemyData.CritMult);
        }
        _activeEnemies.Add(enemy.gameObject);
        if (_isBossFight)
        {
            BossTransform = enemy.transform;
            Damageable bossDamageAble = BossTransform.GetComponent<Damageable>();
            _bossStatusBar.InitializeValue(bossDamageAble.MaxHealth, bossDamageAble.MaxShieldHealth);
            bossDamageAble.OnHealthChanged.AddListener(_bossStatusBar.SetHealth);
            bossDamageAble.OnShieldChanged.AddListener(_bossStatusBar.SetShield);
            BossTransform.GetComponent<EffectsManager>().OnEffectAdded.AddListener(_bossStatusBar.GetComponentInChildren<EffectUIController>().OnEffectAdded);
            BossTransform.GetComponent<EffectsManager>().OnEffectRemoved.AddListener(_bossStatusBar.GetComponentInChildren<EffectUIController>().OnEffectRemoved);
        }
    }

    private EnemyTopDownSettings GetSettingsForId(EnemyId id)
    {
        foreach (var entry in _topDownEnemyWithIdsList)
        {
            if (entry.id == id) return entry.settings;
        }
        return null;
    }

    // ─── Enemy Death Callback ─────────────────────────────────────────────────

    // ─── Cleanup ──────────────────────────────────────────────────────────────

    private void ClearRemainingEnemies()
    {
        foreach (var enemy in _activeEnemies)
        {
            if (enemy != null) enemy.GetComponent<Flyweight>().ReturnToPool();
        }
        _activeEnemies.Clear();
        RemainingEnemiesInWave = 0;
    }

    // ─── Win / Lose ───────────────────────────────────────────────────────────

    private void WinGame()
    {
        ClearRemainingEnemies();
        _gameOver = true;
        EventBus<TopDownEndGameEvent>.Raise(new TopDownEndGameEvent(UIEndGameExecuteState.Win));
    }
}