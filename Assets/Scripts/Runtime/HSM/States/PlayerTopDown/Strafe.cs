using HSM;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Strafe : State
{
    readonly PlayerTopdownContext ctx;
    Vector3 lastLookDir;
    bool _triggeredSkill;
    public Strafe(StateMachine m, State parent, PlayerTopdownContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Add(new LayerWeightActivity(ctx.Animator, ctx.UpperBodyLayerIndex, 1f, 0f, 0.1f)); 
    }
    protected override void OnEnter()
    {
        ctx.TargetMoveSpeed = ctx.StrafeMoveSpeed;
        _triggeredSkill = false;
        ctx.Animator.CrossFade(ctx.StrafeStateHash, 0.1f); // main layer / lower body
        ctx.Animator.CrossFade(ctx.AimAnimName, 0.1f, ctx.UpperBodyLayerIndex);
    }

    protected override void OnUpdate(float deltaTime)
    {
        if (!ctx.IsInSpecialMove)
        {
            ctx.MoveDir = ctx.DesiredMoveDir;
            RotateToMouse(deltaTime);
            return;
        }
        if (!_triggeredSkill)
        {
            _triggeredSkill = true;
            ctx.Animator.CrossFade(ctx.SkillAnimName, 0.1f, ctx.UpperBodyLayerIndex);
        }

        ctx.RotateDir = lastLookDir;
        if (ctx.NeedHoldStill)
        {
            ctx.TargetMoveSpeed = 0;
            ctx.MoveDir = Vector3.zero;
        }
        else ctx.MoveDir = ctx.DesiredMoveDir;
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
    protected override void OnExit()
    {
        ctx.Animator.Play("Empty State", ctx.UpperBodyLayerIndex);
    }
    protected override State GetTransition()
    {
        if (!ctx.IsAiming)
        {
            if (ctx.MoveInput != Vector2.zero)
                return ((Grounded)Parent).Move;
            else
                return ((Grounded)Parent).Idle;
        }
            return null;
    }
}