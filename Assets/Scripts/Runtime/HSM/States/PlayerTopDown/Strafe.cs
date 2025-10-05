using HSM;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Strafe : State
{
    readonly PlayerContext ctx;
    Action getMoveDirByInput;
    public Strafe(StateMachine m, State parent, PlayerContext ctx, Action getMoveDirInput = null) : base(m, parent)
    {
        this.ctx = ctx;
        this.getMoveDirByInput = getMoveDirInput;
        Add(new LayerWeightActivity(ctx.anim, ctx.upperBodyLayerIndex, 1f, 0f, 0.1f)); 
    }
    protected override void OnEnter()
    {
        ctx.targetMoveSpeed = ctx.strafeMoveSpeed;
        ctx.anim.CrossFade(ctx.strafeStateHash, ctx.nextAnimCrossFadeTime); // main layer / lower body

        ctx.anim.CrossFade(ctx.aimType.ToString(), 0.1f,ctx.upperBodyLayerIndex);

    }

    protected override void OnUpdate(float deltaTime)
    {
        UpdateMoveDir();
        RotateToMouse(deltaTime);
    }
    protected override void OnExit()
    {
        ctx.anim.CrossFade("Empty State", ctx.nextAnimCrossFadeTime, ctx.upperBodyLayerIndex);
    }
    private void UpdateMoveDir()
    {
        getMoveDirByInput?.Invoke();
    }
    private void RotateToMouse(float deltaTime)
    {

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPos);

        Plane groundPlane = new Plane(Vector3.up, ctx.rootTransform.position);

        if (groundPlane.Raycast(ray, out float hitDist))
        {
            Vector3 hitPoint = ray.GetPoint(hitDist);

            Vector3 lookDir = hitPoint - ctx.rootTransform.position;
            lookDir.y = 0f; // keep it flat on ground

            if (lookDir.sqrMagnitude > 0.001f)
            {
                ctx.rotateDir = lookDir;
            }
        }
    }
    protected override State GetTransition()
    {
        if (!ctx.isStrafing)
        {
            ctx.nextAnimCrossFadeTime = 0.1f;
            if (ctx.moveInput != Vector2.zero)
                return ((Grounded)Parent).Move;
            else
                return ((Grounded)Parent).Idle;
        }else if (ctx.isAttacking)
        {
            ctx.isStrafing = false; 
            ctx.nextAnimCrossFadeTime = 0.1f;
            return ((Grounded)Parent).Attack;
        }
            return null;
    }
}