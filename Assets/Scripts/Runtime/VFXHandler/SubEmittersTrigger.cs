using UnityEngine;
using System.Collections.Generic;

public class SubEmittersTrigger : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem;

    private List<int> _manualSubEmitterIndices = new List<int>();

    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        CacheManualSubEmitters();
    }

    private void CacheManualSubEmitters()
    {
        _manualSubEmitterIndices.Clear();

        var subEmitters = _particleSystem.subEmitters;

        for (int i = 0; i < subEmitters.subEmittersCount; i++)
        {
            if (subEmitters.GetSubEmitterType(i) == ParticleSystemSubEmitterType.Manual)
            {
                _manualSubEmitterIndices.Add(i);
            }
        }

        Debug.Log($"Found {_manualSubEmitterIndices.Count} Manual sub emitters");
    }

    private void OnParticleTrigger()
    {
        foreach (int index in _manualSubEmitterIndices)
        {
            _particleSystem.TriggerSubEmitter(index);
        }
    }
}