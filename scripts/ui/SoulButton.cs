using Godot;
using ProjectD.scripts.player;

namespace ProjectD.scripts.ui;

public partial class SoulButton : Button
{
    [Export] public Soul Soul;

    [Export] public TextureRect SoulIcon;
    private Vector2 pos;

    public override void _Ready()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        pos = Position;
        if (Soul == null) {
            Visible = false;
            return; }
        SetSoul(Soul);
        
    }

    private void OnMouseExited()
    {
        Scale = Vector2.One;
        AddThemeFontSizeOverride("font_size", 32);
        Position = pos;
    }

    private void OnMouseEntered()
    {
        Scale = Vector2.One * 1.1f;
        AddThemeFontSizeOverride("font_size", 34);
        Position = Vector2.Left * 16; 
    }
    
    public void SetSoul(Soul soul)
    {
        Text = Soul.SoulName;
        TooltipText = Soul.SoulDescription;
        SoulIcon.Texture = Soul.UnitTexture;
        Icon = Soul.SoulTexture;
    }
}