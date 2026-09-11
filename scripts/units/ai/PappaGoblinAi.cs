using Godot;
using System;
using ProjectD.scripts.player;
using ProjectD.scripts.units;

public partial class PappaGoblinAi : Node2D
{
	private enum State { Idle, Move, Attack1, Attack2, Attack3, Attack4, Rest }
	
	[Export] private Unit Pappa;
	[Export] private Sprite2D ArmorSprite, WeaponSprite;
	[Export] private AnimationPlayer Animator;
	[Export] private PackedScene ExplodeBullet;
	[Export] private Soul PappaSause;
	[Export] private float _len = 100f;
	private PlayerController player;
	private Vector2 _weaponOriginalLocalPos;
	private float _weaponOriginalRot;
	float _t=0, _aggressiveness =0, _tiredness = 0;
	private State _state, _toState;
	bool _isResetting = false;

	private static readonly Vector2 ExplodePos = new Vector2(-145, 145), UnderlegExplodePos = new Vector2(-15, 135);
	
	private bool IsBusy => _state is State.Attack1 or State.Attack2 or State.Attack3 or State.Attack4 or State.Rest;

	public override void _Ready()
	{
		Pappa.DamageTaken += PappaOnDamageTaken;
		Animator.AnimationFinished += AnimationFinished;
		
		Pappa.AddSoulStats(PappaSause);
		SetState(State.Idle);
		_weaponOriginalLocalPos = WeaponSprite.Position;
		_weaponOriginalRot = WeaponSprite.Rotation;
		
	}

	private void PappaOnDamageTaken(float amount, int elementType)
	{
		if (Pappa.Armor == 0 && _aggressiveness < .4f) {
			ArmorSprite.Visible = false;
			_aggressiveness += .5f;
			SetState(State.Rest);
		}
		_aggressiveness += .005f;
	}
	
	private void AnimationFinished(StringName animName)
	{
		if (animName == "RESET") {
     			SetState(_toState);
		        return;
		}
		if (animName == "attack2")
			SpawnExplodeBullet(GlobalPosition + ExplodePos);
		
		if (animName == "attack4")
			SpawnExplodeBullet(UnderlegExplodePos + GlobalPosition);

		if ((string)animName is "attack1" or "attack2" or "attack4" or "rest")
			SetState(State.Idle);
		
		
	}

	private void SpawnExplodeBullet(Vector2 targetPos)
	{
		if (ExplodeBullet == null) return;
		var bullet = ExplodeBullet.Instantiate<Bullet>();
		bullet.unit = Pappa;
		bullet.Scale = Vector2.One * 2;
		Game.Instance.Pausable.AddChild(bullet);
		bullet.GlobalPosition = targetPos;
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		

		if (player == null || !IsInstanceValid(player)) {
			if (PlayerStatSaver.Instance.Player != null) player = PlayerStatSaver.Instance.Player;
			if (!IsBusy) SetState(State.Idle);
			return;
		}
		
		var toPlayer = player.GlobalPosition - GlobalPosition;
		var len = toPlayer.Length();
		
		if (IsBusy) {
			Pappa.Velocity = Vector2.Zero;
			Pappa.MoveAndSlide();
			return;
		}
		_t += dt;
		if (_t > 1f) {
			_t = 0;
			_tiredness += 0.1f;

			if (_tiredness > 1.2f) {
				_tiredness = (_aggressiveness > .5f) ? -1 : 0;
				SetState(State.Rest);
				return;
			}
			if (toPlayer is { X: < 0, Y: > 0 }) {
             	SetState(State.Attack2); return;
            }

			if (len > _len*2 && len < _len*3 && GD.Randf() < 0.45f) {
				ThrowHammer(player.GlobalPosition); return;
			}
			
			if (len < _len) {
				SetState(State.Attack4); return;
			}

			if (len <= _len*2) {
				SetState(State.Attack1); return;
			}

			
		}
		if (len > _len*2)
			GoTo(player.GlobalPosition);
		else
			SetState(State.Idle);
		
	}
	private void ThrowHammer(Vector2 targetPos)
	{
		SetState(State.Attack3);

		WeaponSprite.TopLevel = true;

		var startPos = WeaponSprite.GlobalPosition;
		float throwDuration = 0.65f;

		var tween = CreateTween().SetParallel(true);

		tween.TweenProperty(WeaponSprite, "global_position", targetPos, throwDuration)
			.SetTrans(Tween.TransitionType.Quad)
			.SetEase(Tween.EaseType.Out);

		tween.TweenProperty(WeaponSprite, "rotation", WeaponSprite.Rotation + Mathf.Tau * 2.5f, throwDuration);

		tween.Chain().TweenCallback(Callable.From(() =>
		{
			if (!IsInstanceValid(WeaponSprite)) return;
			SpawnExplodeBullet(targetPos);

			WeaponSprite.TopLevel = false;
			WeaponSprite.Position = _weaponOriginalLocalPos;
			WeaponSprite.Rotation = _weaponOriginalRot;

			SetState(State.Idle);
		}));
	}

	void GoTo(Vector2 pos)
	{
		var vector = pos - GlobalPosition;
		Pappa.Velocity = vector.Normalized() * Pappa.Speed;
		Pappa.MoveAndSlide();
		SetState(State.Move);
	}
	
	private void SetState(State newState)
	{
		if (_state == newState) return;
		if (!_isResetting)
		{
			_toState = newState;
			_isResetting = true;
			Animator.Play("RESET");
			return;
		}

		_isResetting = false;
		_state = newState;
		switch (newState)
		{
			case State.Idle:    Animator.Play("idle"); break;
			case State.Move:    Animator.Play("move"); break;
			case State.Attack1: Animator.Play("attack1"); break;
			case State.Attack4: Animator.Play("attack4"); break;
			case State.Attack2: Animator.Play("attack2"); break;
			case State.Attack3: Animator.Play("idle"); break;
			case State.Rest:    Animator.Play("rest"); break;
		}
	}
	
	public override void _ExitTree()
	{
		Pappa.DamageTaken -= PappaOnDamageTaken;
		Animator.AnimationFinished -= AnimationFinished;
	}	
}
