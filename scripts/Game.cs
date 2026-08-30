using Godot;
using System;
using ProjectD.scripts.ui;

public partial class Game : Node2D
{
	public static Game Instance;
	[Export] public Node2D Pausable;
	[Export]public UIController UI; 
	[Export] public Label PausedLabel;
	bool isPaused = false;
	public override void _Ready()
	{
		Instance ??= this;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("esc"))
		{
			isPaused = !isPaused;
			Pausable.ProcessMode = isPaused ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit;
			PausedLabel.Visible = isPaused;
		}
	}

	public void SpawnNode(Node2D node)
	{
		Pausable.AddChild(node);
	}
}
