using Godot;
using System;
using ProjectD.scripts.player;

public partial class SoulContainer : Area2D
{
	[Export] public Soul Soul;
	[Export] public Sprite2D SoulSprite, UnitSprite;
	[Export] CpuParticles2D Particles;
	bool CanObtain = false;
	PlayerController Player;
	public override void _Ready()
	{
		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is not PlayerController pl) return;
		CanObtain = true;
		Player = pl;
	}

	private void OnBodyExited(Node body)
	{
		if (body is not PlayerController pl) return;
		CanObtain = false;
	}

	public override void _Input(InputEvent evt)
	{
		if (!evt.IsActionPressed("f") || !CanObtain) return;
		Player.AddSoul(Soul);
	}
}
