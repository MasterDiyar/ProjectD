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
	private int _patrolPointIndex = 0;
	private float _actionTime = 0f;
	private PlayerController _player;
	private Vector2 _movePos;
	
	
	public ActionType UnitAction = ActionType.Idle;
	public bool IsPlayerClose = false;
	
	public override void _Ready()
	{
		Eyes.BodyEntered += EyeCheck;
		Eyes.BodyExited += EyeExitCheck;
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
		Behavior = UnitBehavior.Aggressive;
		_movePos = _player.Position;
		_player = null;
		if (UnitAction == ActionType.Sleep)  return;
		IsPlayerClose = false;
		UnitAction = ActionType.Move;
		
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		_actionTime += dt;
		if (_actionTime >= BetweenActionTime) {
			switch (Behavior) {
				case UnitBehavior.Patrol:
					if (UnitAction == ActionType.Idle)
						UnitAction = ActionType.Move;
					else if (UnitAction == ActionType.Move) {
						UnitAction = ActionType.Idle;
						_patrolPointIndex = (_patrolPointIndex + 1) % _patrolPoints.Length;
						_movePos = _patrolPoints[_patrolPointIndex];
					} break;
				case UnitBehavior.Animal: case UnitBehavior.Passive:
					if (UnitAction == ActionType.Idle) {
						UnitAction = ActionType.Move;
						_movePos = Position + Vector2.FromAngle(GD.Randf() * Mathf.Tau);
					}
					else if (UnitAction == ActionType.Move)
						UnitAction = ActionType.Idle;
					break;
			}
			if (_unit.Weapon.Resource != null)
				_unit.Weapon.ExecuteShoot((_unit.Position - _player.Position).Angle(), _unit);
			_actionTime = 0f;
		}
		
		MoveBehavior(dt);
		_unit.MoveAndSlide();
	}

	void MoveBehavior(float dt)
	{
		_unit.Velocity = (_unit.Position - _movePos).Normalized() * _unit.Speed;
	}
	
	public override void _ExitTree()
	{
		Eyes.BodyEntered -= EyeCheck;
		Eyes.BodyExited -= EyeExitCheck;
	}
}
