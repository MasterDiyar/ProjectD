 using Godot;
using System;

public partial class FloorSpawner : Node
{
	[Export] Marker2D[] SpawnPoints;
	[Export] private PackedScene[] FloorScenes;
	[Export] private bool IsRepeatable;
	[Export] Node2D WhereToSpawn;
	RandomNumberGenerator rng =  new();
	public override void _Ready()
	{
		rng.Randomize();
		SpawnFloor();
	}

	void SpawnFloor()
	{
		int i=0;
		foreach (Marker2D point in SpawnPoints)
		{
			PackedScene floor;
			floor =(IsRepeatable) ? FloorScenes[rng.Randi() % FloorScenes.Length] : FloorScenes[i++%FloorScenes.Length];
			var room = floor.Instantiate<Node2D>();
			room.Position = point.Position;
			WhereToSpawn.AddChild(room);
			
		}
	}
}
