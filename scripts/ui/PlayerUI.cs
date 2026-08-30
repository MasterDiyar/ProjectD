using System.Linq;
using Godot;
using ProjectD.scripts.player;

namespace ProjectD.scripts.ui;

public partial class PlayerUI : Control
{
    //Souls
    //Hp
    //Mana
    //Shield
    //Regen

    [Export] public VBoxContainer SoulsContainer;
    [Export] public SoulButton[] SoulButtons;

    [Export] public ProgressBar ManaBar, ShieldBar;
    [Export] public HSlider HealthBar;


    public void SetPlayer(PlayerController player)
    {
        player.SoulAdded += AddSoul;
        player.HealthChanged += HealthChanged;
        if (SoulButtons.Length <= 0)
            SoulButtons = SoulsContainer.GetChildren().OfType<SoulButton>().ToArray();
    }

    private void HealthChanged(float currentHp, float maxHp)
    {
        HealthBar.Value = currentHp;
        HealthBar.MaxValue = maxHp;
    }


    public void AddSoul(Soul soul, int where)
    {
        GD.Print(where, SoulButtons.Length);
        if (SoulButtons.Length <= where) return;
        SoulButtons[where].SetSoul(soul);
    }

}