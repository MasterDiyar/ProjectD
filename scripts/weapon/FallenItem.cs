using Godot;
using System;
using ProjectD.scripts.player;

public partial class FallenItem : Area2D
{
	[Export] WeaponResource ItemResource;
	[Export] private Sprite2D texture;
	public override void _Ready()
	{
		if (ItemResource == null) {
			QueueFree();
			return;
		}
		BodyEntered += OnBodyEntered;
		texture.Texture = ItemResource.Texture[0];
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is PlayerController pcr)
		{
			pcr.AddWeapon(ItemResource);
			QueueFree();
		}
	}
}
