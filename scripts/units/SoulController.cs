using Godot;

namespace ProjectD.scripts.units;

public abstract partial class SoulController : Node
{
    [Export] public Soul Soul;
    [Export] public int Location;

    public virtual void OnHpChange(float value, float maxValue)
    { }

    public virtual void OnAttack(float angle)
    { }
    
    
}