using Godot;
using System;
using ProjectD.scripts.player;

public partial class PlayerStatSaver : Node
{
	public static PlayerStatSaver Instance;
	public PlayerController Player;
	public override void _Ready()
	{
		Instance = this;
	}

	public void SaveStats()
	{
		if (Player == null) return;
	}

	public void LoadStats(PlayerController player)
	{
		
	}
	
	
}
