using Godot;
using System;
using ProjectD.scripts.units;

public partial class TargetDummy : Unit
{
	[Export] public Label DamageIndicator;
	private bool _taken = false;
	private float _f;
	public override void _Ready()
	{
		base._Ready();
		DamageTaken += OnDamageTaken;

		
	}

	private void OnDamageTaken(float amount, int elementType)
    {
     	_taken = true;
        _f = 0;
     	DamageIndicator.Visible = true;
     	DamageIndicator.Text = amount.ToString("F2");
     	DamageIndicator.LabelSettings.FontColor = Elements.GetElement((ElementType)elementType).ElementColor;
    }

	public override void _Process(double delta)
	{
		if (!_taken) return;
		_f += (float)delta;
		if (_f < 1.4f) return;
		_f = 0;
		DamageIndicator.Visible = false;
		_taken = false;
	}
}
