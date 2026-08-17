
using Godot;
using WowGd.Src.Physics.Movement.Data;

namespace WowGd.Src.Physics.Movement.BodyPhx;

[GlobalClass]
public partial class BodyAccelerator : Node, IBodyMovement
{
    private const float StillEpsilon = 0.02f;

    // This thing is WIP.
    // The current behavior isn't ideal yet, need more accurate kinematic to avoid perpetual zoomies correction.
    public Vector2 ComputeVelocity(Vector2 position, Vector2 velocity, TargetMove move, double delta)
    {
        float dt = (float)delta;

        Vector2 toTarget = move.Target - position;
        float   d = toTarget.Length();


        if (d < Body.ArrivalEpsilon & velocity.LengthSquared() < StillEpsilon * StillEpsilon)
            return Vector2.Zero;
        
        Vector2 rHat = d > Body.ArrivalEpsilon ? toTarget / d : Vector2.Zero;
        float   vR = velocity.Dot(rHat);
        Vector2 vT = velocity - vR * rHat;

        float radialClosing = Mathf.Max(vR, 0f);
        float dStop = move.Deceleration > 0f
            ? (radialClosing * radialClosing) / (2f * move.Deceleration)
            : 0f;

        float vRDesired = d > dStop
            ? move.MaxSpeed
            : Mathf.Sqrt(Mathf.Max(0f, 2f * move.Deceleration * d));

        Vector2 vDesired    = vRDesired * rHat;
        Vector2 aRaw        = (vDesired - velocity) / dt;

        bool  braking = d <= dStop;
        float bound   = braking
            ? move.Deceleration
            : move.Acceleration;

        Vector2 aRadial;
        if (braking)
        {
            float dSafe   = Mathf.Max(d, Body.ArrivalEpsilon);
            float aNeeded = Mathf.Min(radialClosing * radialClosing / (2f * dSafe), move.Deceleration);
            aRadial = -aNeeded * rHat;   // opposes forward radial motion
        }
        else
        {
            aRadial = vR < move.MaxSpeed ? move.Acceleration * rHat : Vector2.Zero;
        }

        Vector2 aTangentialWanted = -vT / dt;
        float   aTangentialWantedMag = aTangentialWanted.Length();

        Vector2 aTangential;
        float   usedForTangential;
        if (aTangentialWantedMag > bound)
        {
            aTangential = aTangentialWanted / aTangentialWantedMag * bound;
            usedForTangential = bound;
        }
        else
        {
            aTangential = aTangentialWanted;
            usedForTangential = aTangentialWantedMag;
        }

        float remaining = bound - usedForTangential;

        float   aRadialMag  = aRadial.Length();
        Vector2 aRadialUsed = (aRadialMag > remaining && aRadialMag > 0f)
            ? aRadial / aRadialMag * remaining
            : aRadial;

        Vector2 aRadialWanted = (vRDesired - vR) / dt * rHat;
        float   aRadialWantedMag = aRadialWanted.Length();

        Vector2 aFinal = aTangential + aRadialUsed;

        velocity += aFinal * dt;

        if (velocity.LengthSquared() > move.MaxSpeed * move.MaxSpeed && move.MaxSpeed > 0f)
            velocity = velocity.Normalized() * move.MaxSpeed;

        return velocity;
    }
}