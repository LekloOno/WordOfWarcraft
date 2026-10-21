using System;
using CombatCore;
using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Mage;

public sealed class BreachStatus : IDisposable
{
    private Breach? _timeBreach;
    private Breach? _spaceBreach;
    private Breach? _matterBreach;
    private Breach? _lightBreach;
    private readonly IEntity _owner;
    private readonly Authority _authority;

    public Breach? TimeBreach => _timeBreach;
    public Breach? SpaceBreach => _spaceBreach;
    public Breach? MatterBreach => _matterBreach;
    public Breach? LightBreach => _lightBreach;


    public BreachStatus(IEntity owner, Authority authority)
    {
        _owner = owner;
        _authority = authority;

        _timeBreach     = new(GetCollision(), new MatterBreachBuff(), owner);
        _spaceBreach    = new(GetCollision(), new MatterBreachBuff(), owner);
        _matterBreach   = new(GetCollision(), new MatterBreachBuff(), owner);
        _lightBreach    = new(GetCollision(), new MatterBreachBuff(), owner);
    
        //var def = new TypedStatusDefinition()
        //authority.SubmitTypedStatus(owner.Id, new TypedStatusDraft<BreachStatus>(this));
    }

    static CollisionShape2D[] GetCollision()
    {
        CollisionShape2D col = new()
        {
            Shape = new CircleShape2D() { Radius = 1.5f }
        };

        return [col];
    }

    public void Dispose()
    {
        Release(ref _timeBreach);
        Release(ref _spaceBreach);
        Release(ref _matterBreach);
        Release(ref _lightBreach);

        //_authority.RemoveTypedStatus<BreachStatus>(_owner.Id);
    }

    private static void Release(ref Breach? breach)
    {
        breach?.Release();
        breach = null;
    }
}

public enum BreachType
{
    Time,
    Space,
    Matter,
    Light,
}