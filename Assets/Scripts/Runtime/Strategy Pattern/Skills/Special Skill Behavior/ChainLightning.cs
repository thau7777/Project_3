using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ============= CHAIN LIGHTNING SCRIPTABLE OBJECT =============
[CreateAssetMenu(
    fileName = "ChainLightning",
    menuName = "Scriptable Objects/StrategyPattern/Special Skills/Chain Lightning"
)]
public class ChainLightning : SkillStrategy
{
    [TabGroup("Lightning Settings")]
    [SerializeField] private int maxChainTargets = 3;
    [TabGroup("Lightning Settings")]
    [SerializeField] private float chainDistance = 8f;
    [TabGroup("Lightning Settings")]
    [SerializeField] private float chainDelay = 0.15f;
    [TabGroup("Lightning Settings")]
    [SerializeField] private List<EffectData> _effectToApply;


    [TabGroup("Visual Settings")]
    [SerializeField] private OneShotVFXSettings _impactVfxSettings;
    [TabGroup("Visual Settings")]
    [SerializeField] private float arcIntensity = 0.5f;
    [TabGroup("Visual Settings")]
    [SerializeField] private int segmentsPerBolt = 10;

    private SkillStrategyContext _skillContext;

    public override void Execute(IStrategyContext context)
    {
        _skillContext = context as SkillStrategyContext;
        if (_skillContext == null) return;

        MonoBehaviour mono = _skillContext.origin.GetComponent<MonoBehaviour>();
        if (mono != null)
        {
            mono.StartCoroutine(CastChainLightningCoroutine(_skillContext.spawnTransform));
        }
    }

    private IEnumerator CastChainLightningCoroutine(Transform spawnTransform)
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        if (mouseWorldPos == Vector3.zero)
            yield break;

        Transform originAnchor = GetLightningAnchor(spawnTransform.gameObject);

        // Clamp initial range
        Vector3 dir = mouseWorldPos - originAnchor.position;
        dir.y = 0;

        //if (dir.magnitude > Range)
        //{
        //    mouseWorldPos = originAnchor.position + dir.normalized * Range;
        //}

        GameObject firstTarget = FindFirstTargetAtMousePosition(mouseWorldPos);
        if (firstTarget == null)
            yield break;

        List<GameObject> chainedTargets = new List<GameObject>();
        List<ChainLightningController> activeBolts = new List<ChainLightningController>();

        GameObject currentSource = spawnTransform.gameObject;
        GameObject currentTarget = firstTarget;

        for (int i = 0; i < maxChainTargets && currentTarget != null; i++)
        {
            Transform startAnchor = GetLightningAnchor(currentSource);
            if (i == 0)
                startAnchor = currentSource.transform;
            Transform endAnchor = GetLightningAnchor(currentTarget);

            ChainLightningController bolt = CreateLightningBolt(startAnchor, endAnchor);
            activeBolts.Add(bolt);

            ApplyDamage(currentTarget);
            chainedTargets.Add(currentTarget);

            bolt.StartFadeOut(1f);
            yield return Helpers.GetWaitForSeconds(chainDelay);

            currentSource = currentTarget;
            currentTarget = FindNearestEnemy(endAnchor.position, chainedTargets);
        }

        //yield return Helpers.GetWaitForSeconds((_mainSkillVfxSettings as OneShotVFXSettings).DefaultLifeTime);

        //foreach (ChainLightningController bolt in activeBolts)
        //{
        //    if (bolt != null)
        //        bolt.StartFadeOut(1f);
        //}
    }

    // ================== TARGETING ==================

    private GameObject FindFirstTargetAtMousePosition(Vector3 mouseWorldPos)
    {
        Collider[] hits = Physics.OverlapSphere(
            mouseWorldPos,
            indicatorWidth,
            enemyLayer
        );

        GameObject nearest = null;
        float nearestDist = float.MaxValue;

        HashSet<Transform> checkedRoots = new HashSet<Transform>();

        foreach (Collider col in hits)
        {
            Transform root = col.transform.root;
            if (checkedRoots.Contains(root)) continue;

            checkedRoots.Add(root);

            Transform body = root.GetComponentInChildren<SkinnedMeshRenderer>().transform.GetChild(0);
            if (body == null) continue;

            float dist = Vector3.Distance(mouseWorldPos, body.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = body.gameObject;
            }
        }

        return nearest;
    }

    private GameObject FindNearestEnemy(Vector3 fromPosition, List<GameObject> exclude)
    {
        Collider[] hits = Physics.OverlapSphere(
            fromPosition,
            chainDistance,
            enemyLayer
        );

        GameObject nearest = null;
        float nearestDist = float.MaxValue;

        HashSet<Transform> checkedRoots = new HashSet<Transform>();

        foreach (Collider col in hits)
        {
            Transform root = col.transform.root;
            if (checkedRoots.Contains(root)) continue;

            checkedRoots.Add(root);

            Transform body = root.GetComponentInChildren<SkinnedMeshRenderer>().transform.GetChild(0);
            if (body == null) continue;

            if (exclude.Contains(body.gameObject)) continue;

            float dist = Vector3.Distance(fromPosition, body.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = body.gameObject;
            }
        }

        return nearest;
    }

    private Transform GetLightningAnchor(GameObject obj)
    {
        Transform body = obj.transform.root.GetComponentInChildren<SkinnedMeshRenderer>().transform.GetChild(0);
        return body != null ? body : obj.transform;
    }

    // ================== VISUAL ==================

    private ChainLightningController CreateLightningBolt(Transform start, Transform end)
    {
        LineRenderer lr = FlyweightFactory.Spawn(_mainSkillVfxSettings).GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = segmentsPerBolt;
        lr.startWidth = Size;
        lr.endWidth = Size;

        ChainLightningController bolt = lr.gameObject.GetOrAdd<ChainLightningController>();
        bolt.InitializeVFX(Size, 100);
        bolt.InitializeLine(start, end, segmentsPerBolt, arcIntensity);

        return bolt;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
            return hit.point;

        return Vector3.zero;
    }

    private void ApplyDamage(GameObject target)
    {
        var impactVfx = FlyweightFactory.Spawn(_impactVfxSettings);
        impactVfx.FlyweightInitialize(
            GetLightningAnchor(target).position.Add(y:1f),
            Quaternion.identity
        );
        if(impactVfx.TryGetComponent<OneShotVFX>(out var oneShotVFX))
        {
            
            oneShotVFX.InitializeVFX(_impactVfxSettings.DefaultSize, _impactVfxSettings.DefaultLifeTime);
        }
        if (target.transform.root.TryGetComponent<Damageable>(out var damageable))
        {
            Vector3 knockBackDirection = (target.transform.position - _skillContext.origin.position).normalized;
            damageable.TakeDamage(_skillContext.origin.gameObject, null, Damage, DealTrueDamage, knockBackDirection,1,ElementalType.Lightning);
        }
        if(target.transform.root.TryGetComponent<EffectsManager>(out var effectsManager))
        {
            if (_effectToApply == null) return;
            foreach (EffectData effectData in _effectToApply)
            {
                effectsManager.AddEffect(effectData);
            }
        }
    }
}
