using Cysharp.Threading.Tasks;
using PixPlays.ElementalVFX;
using System.Collections.Generic;
using UnityEngine;

public class BeholderHeavenStrike : OneShotVFX
{
    new OneShotVFXSettings settings => (OneShotVFXSettings)base.settings;

    [SerializeField] private OneShotVFXSettings _spellVfxSettings;
    [SerializeField] private SkillIndicatorSettings _indicatorSettings;
    public float indicatorRadius = 5f;
    [SerializeField] private int _numberOfStrikes = 5;
    [SerializeField] private float _radiusAroundPlayer = 7f;
    [SerializeField] private float _minDistanceBetweenStrikes = 1f;
    [SerializeField] private int _delayBeforeStrike = 1;
    [SerializeField] private float _delayBeforeEachStrike = 0.3f;
    [SerializeField] private float _strikeDelay = 1.5f;

    private GameObject _sender;
    private LayerMask _dodgeLayers;
    private int _userAttackDamage;
    private float _skillSize;

    private readonly List<Vector3> _strikePositions = new();

    private void OnEnable()
    {
        _strikePositions.Clear();
        StartStriking().Forget();
    }

    public void SetUp(GameObject sender, LayerMask dodgeLayers, int attackDamage, float skillSize)
    {
        _sender = sender;
        _dodgeLayers = dodgeLayers;
        _userAttackDamage = attackDamage;
        _skillSize = skillSize;
    }

    public async UniTaskVoid StartStriking()
    {
        await UniTask.Delay(_delayBeforeStrike * 1000);

        for (int i = 0; i < _numberOfStrikes; i++)
        {
            Vector3 strikePosition = GetRandomStrikePosition();

            CircleIndicator circleIndicator = FlyweightFactory.Spawn(_indicatorSettings) as CircleIndicator;
            circleIndicator.FlyweightInitialize(strikePosition, Quaternion.identity);
            circleIndicator.Initialize(indicatorRadius, strikePosition);
            circleIndicator.LockIndicator(_strikeDelay);
            circleIndicator.Stop();

            SpawnStrike(strikePosition).Forget();

            await UniTask.Delay((int)(_delayBeforeEachStrike * 1000));
        }
    }

    private async UniTaskVoid SpawnStrike(Vector3 position)
    {
        await UniTask.Delay((int)(_strikeDelay * 1000));

        VfxData data = new(position, position, _spellVfxSettings.DefaultLifeTime, _skillSize, Vector3.zero);
        OneShotVFX strikeVfx = FlyweightFactory.Spawn(_spellVfxSettings) as OneShotVFX;
        strikeVfx.gameObject.SetActive(true);
        strikeVfx.transform.localScale = Vector3.one * _skillSize;
        SetUpHitBox(strikeVfx.gameObject);
        strikeVfx.FlyweightInitialize(position, Quaternion.identity);
        (strikeVfx as LocationVfx).Play(data);
    }

    private Vector3 GetRandomStrikePosition()
    {
        Vector3 candidate = Vector3.zero;
        int maxAttempts = 30;

        do
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            Vector3 randomDirection = new Vector3(randomCircle.x, 0f, randomCircle.y);
            float randomDistance = Random.Range(0, _radiusAroundPlayer);

            candidate = PlayerTopDownStateDriver.Instance.transform.position
                + (randomDirection + PlayerTopDownStateDriver.Instance.transform.forward) * randomDistance;
            candidate.y = 0.1f;

            maxAttempts--;
        }
        while (!IsPositionValid(candidate) && maxAttempts > 0);

        _strikePositions.Add(candidate);
        return candidate;
    }

    private bool IsPositionValid(Vector3 candidate)
    {
        foreach (var pos in _strikePositions)
        {
            Vector3 flat = new Vector3(pos.x, candidate.y, pos.z);
            if (Vector3.Distance(candidate, flat) < _minDistanceBetweenStrikes)
                return false;
        }
        return true;
    }

    private void SetUpHitBox(GameObject vfx)
    {
        if (vfx.TryGetComponent<HitBoxHandler>(out var hitBoxHandler))
        {
            hitBoxHandler.Setup(
                _sender,
                _dodgeLayers,
                _spellVfxSettings.hitboxOnOffTime,
                _spellVfxSettings.useTriggerStays,
                _spellVfxSettings.triggerStayTickInterval,
                false);
        }
        if (vfx.TryGetComponent<DamageDealer>(out var damageDealer))
        {
            damageDealer.Setup(
                _spellVfxSettings.isMagicAttack,
                _userAttackDamage,
                false,
                1,
                false,
                _spellVfxSettings.elementalType,
                _spellVfxSettings.hitImpactVFXSetting);
        }
    }
}