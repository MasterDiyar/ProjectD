using Godot;

[GlobalClass]
public partial class UnitStats : Resource 
{
    [Export] public float BaseHp = 100f;
    [Export] public float BaseArmor = 5f;
    [Export] public float BaseDamage = 10f;
    [Export] public float BaseSpeed = 300f;
    [Export] public float BaseMana = 50f;
    [Export] public float BaseKritChance = 0.1f;
    [Export] public float BaseKritModifier = 1.5f;
}