using System.Collections.Generic;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Physics;

namespace WowGd.Src.Combat.Abilities.Mage;

public partial class Breach : IBody
{
    public Vector2 RawVelocity { get; private set; }
    public Vector2 Inertia { get; private set; }

    public IEnumerable<CollisionShape2D> CollisionShapes => _collisions;

    public IEntity Entity { get => this; set {} }

    public void Accelerate(Vector2 accel) { }
    public void AddRawForce(Vector2 raw) { }
}