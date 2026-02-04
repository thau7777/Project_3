using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CameraShaker : Singleton<CameraShaker>
{
    [SerializeField]
    private CinemachineImpulseSource impulseSource;

    [SerializeField]
    private AnimationCurve customShakeCurve;

    protected override void Awake()
    {
        base.Awake();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        customShakeCurve = impulseSource.ImpulseDefinition.CustomImpulseShape;
    }

    [Button]
    public void GenerateBasicShake()
    {
        ShakeRandomDirection(force: 0.7f, zDirection: -1);
    }
    public void ShakeRandomDirection(CinemachineImpulseDefinition.ImpulseShapes impulseShapes = CinemachineImpulseDefinition.ImpulseShapes.Bump, float force = 1f, float duration = 0.2f, float zDirection = 0f)
    {
        // Generate random direction in XY plane
        Vector3 randomDir = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f),
            zDirection
        ).normalized;
        if(impulseShapes == CinemachineImpulseDefinition.ImpulseShapes.Custom)
        {
            impulseSource.ImpulseDefinition.ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Custom;
            impulseSource.ImpulseDefinition.CustomImpulseShape = customShakeCurve;
        }
        else
            impulseSource.ImpulseDefinition.ImpulseShape = impulseShapes;
        impulseSource.ImpulseDefinition.ImpulseDuration = duration;
        
        impulseSource.GenerateImpulseWithVelocity(randomDir * force);
    }
    //public void ShakeRandomDirection(float force = 1f, CinemachineImpulseDefinition.ImpulseShapes impulseShapes = CinemachineImpulseDefinition.ImpulseShapes.Bump)
    //{
    //    // Generate random direction in XY plane
    //    Vector3 randomDir = new Vector3(
    //        Random.Range(-1f, 1f),
    //        Random.Range(-1f, 1f),
    //        0f
    //    ).normalized;
    //    //impulseSource.ImpulseDefinition.ImpulseShape = impulseShapes;
    //    impulseSource.GenerateImpulseWithVelocity(randomDir * force);
    //}
}
