using Godot;
using System;
using ProjectD.scripts.items;
using ProjectD.scripts.player;

public partial class FallenItem : PickupItem
{
	[Export] public WeaponResource ItemResource;
	[Export] private Sprite2D texture;
	public override void _Ready()
	{
		base._Ready();
		if (ItemResource != null)
			SetItem(ItemResource);
	}
	protected void SetTexture() => texture.Texture = ItemResource.Texture[0];

	protected override void Pickup()
	{
		Player.AddWeapon(ItemResource);
		QueueFree();
	}

	public virtual void SetItem(WeaponResource res)
	{
		ItemResource = res;
		SetTexture();
	}
}