using Godot;
using System;
using ProjectD.scripts.player;

public partial class LockedItem : FallenItem
{
	[Export] public MoneyManager.CoinType Type;
	[Export] public float Cost;
	private bool CanBuy = false;
	PlayerController player;
	[Export] Label label;
	public override void _Ready()
	{
		base._Ready();
		BodyExited += OnBodyExited;
		label.Text = $"{ItemResource.ResourceName}\nCost: {Cost}, {Type}";
	}

	private void OnBodyExited(Node2D body)
	{
		CanBuy = false;
		label.Visible = false;
	}

	protected override void OnBodyEntered(Node2D body)
	{
		if (body is not PlayerController prc) return;
		player = prc;
		label.Visible = true;
		CanBuy = (MoneyManager.Instance.CanSpendCoin(Type, Cost));
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("f") && CanBuy)
			AddWeapon(player);
	}
}
