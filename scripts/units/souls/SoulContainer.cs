using Godot;
using System;
using ProjectD.scripts.items;
using ProjectD.scripts.player;

public partial class SoulContainer : PickupItem
{
	[Export] public Soul Soul;
	[Export] public Sprite2D SoulSprite, UnitSprite;
	[Export] CpuParticles2D Particles;

	public override void _Ready()
	{
		base._Ready();
		SoulSprite.Texture = Soul.SoulTexture;
		UnitSprite.Texture = Soul.UnitTexture;
	}

	protected override void Pickup()
	{
		Player.AddSoul(Soul);
		QueueFree();
	}
}
