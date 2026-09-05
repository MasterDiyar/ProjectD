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
		{
			MoneyManager.Instance.SpendCoin(Type, Cost);
			AddWeapon(player);
		}
	}

	public override void SetItem(WeaponResource res)
	{
		base.SetItem(res);
		if (label == null || IsInstanceValid(label)) label = GetNode<Label>("Label");
		label.Text = $"{ItemResource.ResourceName}\nCost: {Cost}, {Type}";
	}
}
