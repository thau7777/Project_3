using HSM;
using UnityEngine;

public class Strafe : State
{
    readonly PlayerContext ctx;

    public Strafe(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
    {
        this.ctx = ctx;
        Add(new ColorPhaseActivity(ctx.renderer)
        {
            enterColor = Color.red, // runs while Airborne is activating
        });
    }
    protected override void OnEnter()
    {
        ctx.targetMoveSpeed = 2;
    }

    protected override void OnUpdate(float deltaTime)
    {
        Ray ray = Camera.main.ScreenPointToRay(ctx.mouseScreenPos);

        // Step 3: define a plane (XZ ground at player's root position)
        Plane groundPlane = new Plane(Vector3.up, ctx.rootTransform.position);

        if (groundPlane.Raycast(ray, out float hitDist))
        {
            Vector3 hitPoint = ray.GetPoint(hitDist);

            // Step 4: get direction from player to hit point
            Vector3 lookDir = hitPoint - ctx.rootTransform.position;
            lookDir.y = 0f; // keep it flat on ground

            if (lookDir.sqrMagnitude > 0.001f)
            {
                // Step 5: smoothly rotate toward that direction
                Quaternion targetRot = Quaternion.LookRotation(lookDir);
                ctx.rootTransform.rotation = Quaternion.Slerp(
                    ctx.rootTransform.rotation,
                    targetRot,
                    deltaTime * 10f // tweak speed
                );
            }
        }
    }

    protected override State GetTransition()
    {
        if (!ctx.isAiming)
        {
            if(ctx.moveInput != Vector2.zero)
                return ((Grounded)Parent).Move;
            else
                return ((Grounded)Parent).Idle;
        }
        return null;
    }
}