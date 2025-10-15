using HSM;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Strafe : State
{
    readonly PlayerContext ctx;
    Action getMoveDirByInput;
    Vector3 lastLookDir;
    public Strafe(StateMachine m, State parent, PlayerContext ctx, Action getMoveDirInput = null) : base(m, parent)
    {
        this.ctx = ctx;
        this.getMoveDirByInput = getMoveDirInput;
        Add(new LayerWeightActivity(ctx.Animator, ctx.UpperBodyLayerIndex, 1f, 0f, 0.1f)); 
    }
    protected override void OnEnter()
    {
        ctx.TargetMoveSpeed = ctx.StrafeMoveSpeed;
        ctx.Animator.CrossFade(ctx.StrafeStateHash, ctx.NextAnimCrossFadeTime); // main layer / lower body
    }

    protected override void OnUpdate(float deltaTime)
    {
        if (!ctx.IsInSpecialMove)
        {
            UpdateMoveDir();
            RotateToMouse(deltaTime);
            return;
        }
        ctx.RotateDir = lastLookDir;
        if (ctx.NeedHoldStill)
        {
            ctx.TargetMoveSpeed = 0;
            ctx.MoveDir = Vector3.zero;
        }
        else UpdateMoveDir();
    }
    protected override void OnExit()
    {
        if(!ctx.IsAttacking)
        ctx.Animator.CrossFade("Empty State", ctx.NextAnimCrossFadeTime, ctx.UpperBodyLayerIndex);
    }
    private void UpdateMoveDir()
    {
        getMoveDirByInput?.Invoke();
    }
    private void RotateToMouse(float deltaTime)
    {

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

        Plane groundPlane = new Plane(Vector3.up, ctx.RootTransform.position);

        if (groundPlane.Raycast(ray, out float hitDist))
        {
            Vector3 hitPoint = ray.GetPoint(hitDist);

            Vector3 lookDir = hitPoint - ctx.RootTransform.position;
            lookDir.y = 0f; // keep it flat on ground

            if (lookDir.sqrMagnitude > 0.001f)
            {
                ctx.RotateDir = lookDir;
                lastLookDir = lookDir;
            }
        }
    }
    protected override State GetTransition()
    {
        if (!ctx.IsStrafing)
        {
            ctx.NextAnimCrossFadeTime = 0.1f;
            if (ctx.MoveInput != Vector2.zero)
                return ((Grounded)Parent).Move;
            else
                return ((Grounded)Parent).Idle;
        }else if (ctx.IsAttacking)
        {
            ctx.IsStrafing = false;
            ctx.NextAnimCrossFadeTime = 0.1f;
            return ((Grounded)Parent).Attack;
        }
            return null;
    }
}