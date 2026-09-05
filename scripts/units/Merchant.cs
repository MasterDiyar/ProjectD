using Godot;
using System;
using ProjectD.scripts.player;

public partial class Merchant : Node2D
{
	//prototype ver1
	//Just animation of stacking items and pseudo random items on sell
	
	//prototype ver2
	//will be more accurate and will haev accesoriries and code

	[Export] public WeaponResource[] WeaponPool;
	[Export] public LockedItem[] LockedItems;
	[Export] public Area2D Eyes;
	[Export] public AnimationPlayer AnimationPlayer;
	private float _timeBetweenActions = 0;
	bool _Started = false, _been = false;
	int _currentIndex = 0;
	public override void _Ready()
	{
		Eyes.BodyEntered += EyesOnBodyEntered;
	}

	private void EyesOnBodyEntered(Node2D body)
	{
		if (body is not PlayerController pcr) return;
		if (_been) return;
		MoneyManager.Instance.AddCoins(MoneyManager.CoinType.GoldePrusse, 400);
		_been = true;
		_Started = true;
		AnimationPlayer.Play("GiveTiers");
	}


	public override void _Process(double delta)
	{
		if (!_Started) return;
		
		if (LockedItems == null || LockedItems.Length == 0 || WeaponPool == null || WeaponPool.Length == 0) {
			_Started = false;
			return;
		}

		_timeBetweenActions += (float)delta;
		if (_timeBetweenActions < 0.75f) return;

		if (_currentIndex < LockedItems.Length && _currentIndex < 4) {
			int randomWeaponIdx = (int)(GD.Randi() % (uint)WeaponPool.Length);
			LockedItems[_currentIndex].SetItem(WeaponPool[randomWeaponIdx]);
			_currentIndex++;
			_timeBetweenActions = 0;
		}

		if (_currentIndex >= 4 || _currentIndex >= LockedItems.Length)
			_Started = false;
		
	}
}
