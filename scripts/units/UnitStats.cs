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

    public static UnitStats operator +(UnitStats a, UnitStats b)
    {
        if (a is null) return b is null ? new UnitStats() : Clone(b);
        if (b is null) return Clone(a);
        return new UnitStats
        {
            BaseHp = a.BaseHp + b.BaseHp,
            BaseArmor = a.BaseArmor + b.BaseArmor,
            BaseDamage = a.BaseDamage + b.BaseDamage,
            BaseSpeed = a.BaseSpeed + b.BaseSpeed,
            BaseMana = a.BaseMana + b.BaseMana,
            BaseKritChance = a.BaseKritChance + b.BaseKritChance,
            BaseKritModifier = a.BaseKritModifier + b.BaseKritModifier
        };
    }

    public static UnitStats Clone(UnitStats a) => new()
        {
            BaseHp = a.BaseHp,
            BaseArmor = a.BaseArmor,
            BaseDamage = a.BaseDamage,
            BaseSpeed = a.BaseSpeed,
            BaseMana = a.BaseMana,
            BaseKritChance = a.BaseKritChance,
            BaseKritModifier = a.BaseKritModifier,
        };
    
    public void Add(UnitStats other)
    {
        if (other is null) return;

        BaseHp += other.BaseHp;
        BaseArmor += other.BaseArmor;
        BaseDamage += other.BaseDamage;
        BaseSpeed += other.BaseSpeed;
        BaseMana += other.BaseMana;
        BaseKritChance += other.BaseKritChance;
        BaseKritModifier += other.BaseKritModifier;
    }

    public static UnitStats Zero => new()
    {
        BaseHp = 0, BaseArmor = 0, BaseDamage = 0,
        BaseSpeed = 0, BaseMana = 0, BaseKritChance = 0, BaseKritModifier = 0
    };
    
}