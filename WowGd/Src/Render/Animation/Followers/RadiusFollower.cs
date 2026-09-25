using Godot;
using WowGd.Src.Render.Animation.Smoothing;

namespace WowGd.Src.Render.Animation.Followers;

[GlobalClass]
public partial class RadiusFollower : Node2DFollower
{
	[Export] private float _radius              = 1f;
	[Export] private float _lookTriggerRadius   = 0.9f;
	[Export] private float _lookDistance        = 1.4f;
	private Vector2 _targetCenter;
	private Vector2 _targetLook;

	private PositionVelDamperData   _centerVelDamperData = null!;
	private PositionVelDamper       _center = null!;

	[Export] public PositionVelDamperData CenterVelDamperData
	{
		get => _centerVelDamperData;
		set
		{
			if (value == _centerVelDamperData)
				return;

			_centerVelDamperData = value;

			if (_center != null)
				_center.Data = value;
		}    
	}

	private PositionVelDamperData   _lookVelDamperData = null!;
	private PositionVelDamper       _look = null!;

	[Export] public PositionVelDamperData LookVelDamperData
	{
		get => _lookVelDamperData;
		set
		{
			if (value == _lookVelDamperData)
				return;

			_lookVelDamperData = value;

			if (_look != null)
				_look.Data = value;
		}    
	}

	protected override void ReadySpec()
	{
		_center = new(_centerVelDamperData);
		_look   = new(_lookVelDamperData);
	}

	private Vector2 _previousTargetPosition;
	private Vector2 _currentTargetPosition;
	public override void _PhysicsProcess(double delta)
	{
		_previousTargetPosition = _currentTargetPosition;
		_currentTargetPosition = _node.GlobalPosition;
	}

	public override void _Process(double delta)
	{
		float alpha     = (float)Engine.GetPhysicsInterpolationFraction();

		Vector2 targetPosition =
			_previousTargetPosition.Lerp(_currentTargetPosition, alpha);

		Vector2 posDelt = targetPosition - _targetCenter;
		Vector2 dir     = posDelt.Normalized();

		float distance  = posDelt.Length();
		float offset    = distance - _radius;
		float dt        = (float) delta;


		
		if (offset > 0f)
			_targetCenter += dir * offset;

		_center.Process(_targetCenter, dt);

		Vector2 lookDir     = targetPosition - _center.Position;
		float realDistance  = lookDir.Length();
		if (realDistance > _lookTriggerRadius)
			_targetLook = lookDir.Normalized() * _lookDistance;

		_look.Process(_targetLook, dt);
		
		GlobalPosition = _center.Position + _look.Position;
	}
}
