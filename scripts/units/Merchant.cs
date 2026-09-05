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
	bool _Started = false;
	int _i = 0;
	public override void _Ready()
	{
		Eyes.BodyEntered += EyesOnBodyEntered;
	}

	private void EyesOnBodyEntered(Node2D body)
	{
		if (body is not PlayerController pcr) return;
		_Started = true;
		AnimationPlayer.Play("GiveTiers");
	}


	public override void _Process(double delta)
	{
		if (!_Started) return;
		
		_timeBetweenActions+=(float)delta;
		if (!(_timeBetweenActions >= 0.75f)) return;
		
		LockedItems[_i++].ItemResource = WeaponPool[GD.Randi()%WeaponPool.Length];
		_timeBetweenActions = 0;
		if (_i == 4 || _i == LockedItems.Length) _Started = false;
	}
}
