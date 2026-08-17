using Godot;

namespace WowGd.Src.Physics.General;

public static class Tools
{
    public static bool TryGetMouseWorldPosition(
        out Vector3 position,
        Viewport    vp,
        uint        collisionMask = 4294967295,
        Camera3D?   camera = null)
    {
        position = Vector3.Zero;

        camera ??= vp.GetCamera3D();

        Vector2 mousePosition = vp.GetMousePosition();

        if (camera == null)
            return false;

        Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
        Vector3 rayDirection = camera.ProjectRayNormal(mousePosition);

        const float rayLength = 1000.0f;

        PhysicsRayQueryParameters3D query =
            PhysicsRayQueryParameters3D.Create(
                rayOrigin,
                rayOrigin + rayDirection * rayLength,
                collisionMask
            );

        PhysicsDirectSpaceState3D spaceState =
            camera.GetWorld3D().DirectSpaceState;

        Godot.Collections.Dictionary result =
            spaceState.IntersectRay(query);

        if (result.Count == 0)
            return false;

        position = (Vector3)result["position"];
        return true;
    }
}