using System.Collections.Generic;
using Godot;
using WowGd.Src.Physics;

namespace WowGd.Src.Tools;

public static class PhysicsExt
{
    public static bool IsContactWithin(CollisionShape2D a, CollisionShape2D b, float threshold)
        => IsContactWithin(a.Shape, a.GlobalTransform, b.Shape, b.GlobalTransform, threshold);

    public static bool IsContactWithin(IBody a, IBody b, float threshold)
    {
        foreach (var sa in a.CollisionShapes)
            foreach (var sb in b.CollisionShapes)
                if (IsContactWithin(sa, sb, threshold))
                    return true;
        return false;
    }

    public static bool IsContactWithin(Shape2D sa, Transform2D xa, Shape2D sb, Transform2D xb, float threshold)
    {
        Vector2 delta = xb.Origin - xa.Origin;
        float distSq = delta.LengthSquared();

        // Coincident origins - no physics check required
        if (distSq < 1e-8f) return true;

        // Already touching/overlapping
        if (sa.Collide(xa, sb, xb)) return true;

        float dist = Mathf.Sqrt(distSq);
        float t = Mathf.Min(threshold, dist);

        Transform2D moved = xa;
        moved.Origin += delta / dist * t;
        return sa.Collide(moved, sb, xb);
    }

    public static bool IsProcessTick(uint mask, uint id) =>
        (mask & Engine.GetPhysicsFrames()) ==
        (mask & id);
}