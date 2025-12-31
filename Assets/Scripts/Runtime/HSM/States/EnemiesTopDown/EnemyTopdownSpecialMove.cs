using UnityEngine;
using HSM;
public class EnemyTopdownSpecialMove : State
{
    public enum SpecialMoveState
    {
        Charging,
        Executing,
        Recovering,
        Finished
    }

    readonly EnemyTopdownContext ctx;

    private SpecialMoveState _currentSpecialMoveState;
    private float _elapsedTime;
    private Vector3 _targetLastPosition;

    public EnemyTopdownSpecialMove(StateMachine machine, State parent, EnemyTopdownContext context) : base(machine, parent)
    {
        ctx = context;
    }

    protected override void OnEnter()
    {

        _elapsedTime = 0;
        ctx.CurrentSpeed = ctx.EnemySpecialMoveData.movementSpeed;
        _targetLastPosition = ctx.CurrentTargetTransform.position;
        ctx.BossAttackCoolDownTimer = 0;

        bool needCharge = ctx.EnemySpecialMoveData.chargeDuration != 0;
        _currentSpecialMoveState = needCharge ? SpecialMoveState.Charging : SpecialMoveState.Executing;
        ctx.Animator.CrossFade(needCharge ? ctx.EnemySpecialMoveData.chargeAnimationName : ctx.EnemySpecialMoveData.executeAnimationName, ctx.EnemySpecialMoveData.crossfadeDuration, 0);
        
    }
    protected override void OnUpdate(float deltaTime)
    {
        UpdateCurrentState(deltaTime);
        UpdateMoveDir();
    }
    private void UpdateMoveDir()
    {
        if (Vector3.Distance(ctx.RootTransform.position, ctx.CurrentTargetTransform.position) < 0.1f)
            return;
        ctx.MoveDir = (ctx.CurrentTargetTransform.position - ctx.RootTransform.position).normalized;
    }
    private void UpdateCurrentState(float deltaTime)
    {
        _elapsedTime += deltaTime;
        if (ctx.EnemySpecialMoveData.faceTargetState == EnemySpecialMoveData.FaceTargetState.Full)
            ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime, ctx.CurrentTargetTransform.position);
        switch (_currentSpecialMoveState)
        {
            case SpecialMoveState.Charging:
                {
                    if (ctx.EnemySpecialMoveData.faceTargetState == EnemySpecialMoveData.FaceTargetState.ChargeOnly)
                        ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime, ctx.CurrentTargetTransform.position);

                    if (_elapsedTime >= ctx.EnemySpecialMoveData.chargeDuration)
                    {
                        _elapsedTime = 0;
                        _currentSpecialMoveState = SpecialMoveState.Executing;
                        ctx.Animator.CrossFade(ctx.EnemySpecialMoveData.executeAnimationName, ctx.EnemySpecialMoveData.crossfadeDuration);
                    }
                    break;
                }
            case SpecialMoveState.Executing:
                {
                    if (ctx.EnemySpecialMoveData.faceTargetState == EnemySpecialMoveData.FaceTargetState.ExecutingOnly)
                        ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime, ctx.CurrentTargetTransform.position);
                    if (_elapsedTime >= ctx.EnemySpecialMoveData.executionDuration)
                    {
                        _elapsedTime = 0;
                        _currentSpecialMoveState = SpecialMoveState.Recovering;
                        ctx.Animator.CrossFade(ctx.IdleHash, ctx.EnemySpecialMoveData.recoveryDuration);
                    }
                    break;
                }
            case SpecialMoveState.Recovering:
                {
                    if (ctx.EnemySpecialMoveData.faceTargetState == EnemySpecialMoveData.FaceTargetState.Recovering)
                        ((EnemyTopdownRoot)Parent).UpdateRotation(deltaTime, ctx.CurrentTargetTransform.position);
                    if (_elapsedTime >= ctx.EnemySpecialMoveData.recoveryDuration)
                    {
                        _elapsedTime = 0;
                        _currentSpecialMoveState = SpecialMoveState.Finished;
                    }
                        
                    break;
                }
        }

    }
    
    protected override void OnExit()
    {
        ctx.CurrentSpeed = ctx.BaseMoveSpeed;
    }
    protected override State GetTransition()
    {
        if (ctx.IsDead)
        {
            return ((EnemyTopdownRoot)Parent).Dead;
        }
        if (ctx.IsStunned)
        {
            ctx.IsInSpecialMove = false;
            return ((EnemyTopdownRoot)Parent).Stunned;
        }
        if (_currentSpecialMoveState == SpecialMoveState.Finished)
        {
            ctx.IsInSpecialMove = false;
            return ((EnemyTopdownRoot)Parent).Idle;
        }
        return null;
    }
}
