using Godot;
using System;
using ProjectD.scripts.player;

public partial class LockedItem : FallenItem
{
	[Export] public MoneyManager.CoinType Type;
	[Export] public float Cost;
	[Export] Label label;

	protected override void OnBodyEntered(Node2D body)
	{
		base.OnBodyEntered(body);
		if (body is not PlayerController) return;
		label.Visible = true;
	}

	protected override void OnBodyExited(Node2D body)
	{
		base.OnBodyExited(body);
		label.Visible = false;
	}

	protected override bool CanPickup() =>
		MoneyManager.Instance.CanSpendCoin(Type, Cost);

	protected override void Pickup()
	{
		MoneyManager.Instance.SpendCoin(Type, Cost);
		base.Pickup();
	}

	public override void SetItem(WeaponResource res)
	{
		base.SetItem(res);
		label ??= GetNode<Label>("Label");
		label.Text = $"{ItemResource.ResourceName}\nCost: {Cost}, {Type}";
	}
}
