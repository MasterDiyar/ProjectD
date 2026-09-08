using Godot;
using ProjectD.scripts.player;

namespace ProjectD.scripts.items;

public abstract partial class PickupItem : Area2D
{
    protected PlayerController Player;
    protected bool PlayerInRange = false;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    protected virtual void OnBodyEntered(Node2D body)
    {
        if (body is not PlayerController pl) return;
        Player = pl;
        PlayerInRange = true;
    }

    protected virtual void OnBodyExited(Node2D body)
    {
        if (body is not PlayerController) return;
        PlayerInRange = false;
    }

    public override void _Input(InputEvent evt)
    {
        if (!evt.IsActionPressed("f") || !PlayerInRange) return;
        if (!CanPickup()) return;
        Pickup();
    }

    protected virtual bool CanPickup() => true;

    protected abstract void Pickup();
}