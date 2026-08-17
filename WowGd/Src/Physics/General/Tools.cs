using Godot;

namespace WowGd.Src.Physics.General;

public static class Tools
{
    /// <summary>
    /// Try to solve the mouse cursor position to a 3D world position, by raycasting from the camera to any collision.
    /// </summary>
    /// <param name="position">
    /// The resulting world global position.
    /// Might be irrelevant if the returning value is false.
    /// </param>
    /// <param name="vp">
    /// The viewport to compute this from, can be obtained from any Godot node from `GetViewPort()` method.
    /// </param>
    /// <param name="collisionMask">
    /// An optionnal specific collision mask to use. Resolves to all-included otherwise.
    /// </param>
    /// <param name="camera">
    /// An optionnal specific camera to use. Resolves to the current active camera otherwise.
    /// </param>
    /// <returns>
    /// Whether a relevant position could be computed.
    /// </returns>
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