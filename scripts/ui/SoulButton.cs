using Godot;
using ProjectD.scripts.player;

namespace ProjectD.scripts.ui;

public partial class SoulButton : Button
{
    [Export] public Soul Soul;

    [Export] public Label SoulName;
    [Export] public TextureRect SoulIcon;

    public override void _Ready()
    {
        SoulName.Text = Soul.SoulName;
        TooltipText = Soul.SoulDescription;
        SoulIcon.Texture = Soul.UnitTexture;
        Icon = Soul.SoulTexture;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private void OnMouseExited()
    {
        Scale = Vector2.One;
    }

    private void OnMouseEntered()
    {
        Scale = Vector2.One * 1.1f;
    }
}