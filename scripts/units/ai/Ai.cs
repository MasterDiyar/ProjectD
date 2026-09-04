using Godot;
using System;
using ProjectD.scripts.player;
using ProjectD.scripts.units;
using ProjectD.scripts.weapon;

public enum UnitBehavior
{
	Animal,
	Passive,
	PassiveAnimal,
	Aggressive,
	Patrol
}
public enum ActionType
{
 	Move,
 	Attack,
 	Idle,
 	Sleep
}

public partial class Ai : Node2D
{
	[Export] private Unit _unit;
	[Export] public UnitBehavior Behavior;
	[Export] public float BetweenActionTime = 2.5f;
	[Export] public Area2D Eyes;
	

	[Export] private Vector2[] _patrolPoints = [];
	[Export] public bool Overrider = false;
	private int _patrolPointIndex = 0;
	private float _actionTime = 0f;
	private PlayerController _player;
	private Vector2 _movePos;
	private UnitBehavior prevBehavior;
	
	public ActionType UnitAction = ActionType.Idle;
	public bool IsPlayerClose = false;
	
	public override void _Ready()
	{
		if (Overrider)
			foreach (var n in GetParent().GetChildren())
				if (n is Ai { Overrider: false } ai) {
					Eyes = ai.Eyes;
					ai.QueueFree();
				}
		
		Eyes.BodyEntered += EyeCheck;
		Eyes.BodyExited += EyeExitCheck;
		_unit.DamageTaken += Damaged;
		
		_movePos = _unit.GlobalPosition;
	}

	void Damaged(float damage, int f)
	{
		if (UnitAction == ActionType.Sleep) UnitAction = ActionType.Idle;
		var pP = _player?.GlobalPosition ?? Vector2.Zero;
		var moveVector = 100 * (_unit.GlobalPosition - pP).Normalized();
		if (Behavior is UnitBehavior.PassiveAnimal or UnitBehavior.Passive) {
			UnitAction = ActionType.Move;
			_movePos = _unit.GlobalPosition + moveVector;
		}else {
			UnitAction = ActionType.Attack;
			_movePos = _unit.GlobalPosition - moveVector;
		}
		_actionTime = 0f;
	}

	private void EyeCheck(Node2D body)
	{
		if (body is not PlayerController pl) return;
		_player = pl;
		if (UnitAction == ActionType.Sleep)  return;
		IsPlayerClose = true;
		UnitAction = ActionType.Attack;
	}

	private void EyeExitCheck(Node2D body)
	{
		if (body is not PlayerController pl) return;
		if (Behavior == UnitBehavior.PassiveAnimal) {
			_player = null;
			return;
		}
		if (_player != null)
			_movePos = _player.GlobalPosition;
		_player = null;
		if (UnitAction == ActionType.Sleep)  return;
		Behavior = UnitBehavior.Aggressive;
		IsPlayerClose = false;
		UnitAction = ActionType.Move;
		
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		_actionTime += dt;
		if (_actionTime >= BetweenActionTime) 
			Think();
		
		if (_player != null && _unit.Weapon?.Resource != null && UnitAction == ActionType.Attack) 
			_unit.Weapon.ExecuteShoot((_player.GlobalPosition - _unit.GlobalPosition).Angle(), _unit);
		
		MoveBehavior();
		_unit.MoveAndSlide();
	}

	void Think()
	{
		if (GD.Randf() > 0.5f && UnitAction is ActionType.Idle or ActionType.Move)
			UnitAction = UnitAction == ActionType.Idle ? ActionType.Move : ActionType.Idle;
		switch (Behavior) {
			case UnitBehavior.Patrol:
				if (_patrolPoints.Length <= 0) break;
				_patrolPointIndex = (_patrolPointIndex + 1) % _patrolPoints.Length;
				_movePos = _patrolPoints[_patrolPointIndex];
				break;
			case UnitBehavior.Animal: case UnitBehavior.Passive:
				_movePos = _unit.GlobalPosition + 50 * Vector2.FromAngle(GD.Randf() * Mathf.Tau);
				break;
		}
			
		_actionTime = 0f;
	}

	void MoveBehavior()
	{
		if (UnitAction is ActionType.Move or ActionType.Attack)
		{
			if (_unit.GlobalPosition.DistanceSquaredTo(_movePos) > 10f) 
				_unit.Velocity = (_movePos - _unit.GlobalPosition).Normalized() * _unit.Speed;
			else {
				_unit.Velocity = Vector2.Zero;
				if (UnitAction == ActionType.Move && Behavior != UnitBehavior.Patrol) 
					UnitAction = ActionType.Idle;
				if (Behavior == UnitBehavior.Aggressive && _player != null)
					Behavior = UnitBehavior.Passive;
			}
		}
		else _unit.Velocity = Vector2.Zero;
		
	}
	
	public override void _ExitTree()
	{
		Eyes.BodyEntered -= EyeCheck;
		Eyes.BodyExited -= EyeExitCheck;
		if (_unit != null) 
			_unit.DamageTaken -= Damaged;
	}
}
