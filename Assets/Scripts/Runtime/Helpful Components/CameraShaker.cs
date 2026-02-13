using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CameraShaker : Singleton<CameraShaker>
{
    [SerializeField]
    private CinemachineImpulseSource impulseSource;

    [SerializeField]
    private List<AnimationCurve> customShakeCurveList = new();

    protected override void Awake()
    {
        base.Awake();
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    [Button]
    public void GenerateBasicShake()
    {
        ShakeRandomDirection(force: 0.7f);
    }
    public void ShakeRandomDirection(CinemachineImpulseDefinition.ImpulseShapes impulseShapes = CinemachineImpulseDefinition.ImpulseShapes.Bump, float force = 1f, float duration = 0.2f, int customShapeIndex = 0)
    {
        // Generate random direction in XY plane
        Vector3 randomDir = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            -1f
        ).normalized;
        if(impulseShapes == CinemachineImpulseDefinition.ImpulseShapes.Custom)
        {
            impulseSource.ImpulseDefinition.ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Custom;
            impulseSource.ImpulseDefinition.CustomImpulseShape = customShakeCurveList[customShapeIndex];
        }
        else
            impulseSource.ImpulseDefinition.ImpulseShape = impulseShapes;
        impulseSource.ImpulseDefinition.ImpulseDuration = duration;
        impulseSource.GenerateImpulseWithVelocity(randomDir * force);
    }
    public void ShakeByDirection(Vector3 direction, CinemachineImpulseDefinition.ImpulseShapes impulseShapes = CinemachineImpulseDefinition.ImpulseShapes.Bump, float duration = 0.2f, int customShapeIndex = 0)
    {
        if(impulseShapes == CinemachineImpulseDefinition.ImpulseShapes.Custom)
        {
            impulseSource.ImpulseDefinition.ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Custom;
            impulseSource.ImpulseDefinition.CustomImpulseShape = customShakeCurveList[customShapeIndex];
        }
        else
            impulseSource.ImpulseDefinition.ImpulseShape = impulseShapes;
        impulseSource.ImpulseDefinition.ImpulseDuration = duration;
        impulseSource.GenerateImpulseWithVelocity(direction);
    }
}
