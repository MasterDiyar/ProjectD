using Godot;
using System;
using ProjectD.scripts.player;

public partial class PlayerStatSaver : Node
{
	public static PlayerStatSaver Instance;
	public override void _Ready()
	{
		Instance = this;
	}

	public void SaveStats()
	{
		
	}

	public void LoadStats(PlayerController player)
	{
		
	}
	
	
}
