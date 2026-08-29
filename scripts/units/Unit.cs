using Godot;
using ProjectD.scripts.player;
using ProjectD.scripts.weapon;

namespace ProjectD.scripts.units;

public partial class Unit : CharacterBody2D
{
    [Signal] public delegate void HealthChangedEventHandler(float currentHp, float maxHp);
    [Signal] public delegate void DamageTakenEventHandler(float amount, int elementType);
    [Signal] public delegate void HealedEventHandler(float amount);
    [Signal] public delegate void StatsLoadedEventHandler();
    [Export]public UnitStats Stats;
    [Export] public Weapon Weapon;
    public float Hp, MaxHp;
    public float Armor, MaxArmor;
    public float Damage;
    public float Speed, MaxSpeed;
    public float Mana, MaxMana;
    public float KritChance;
    public float KritModifier;
    
    public ElementType UnitElement, HittedElement;
    public WeaponType UnitWeapon;

    public override void _Ready()
    {
        MaxHp = Stats.BaseHp;
        MaxArmor = Stats.BaseArmor;
        Damage = Stats.BaseDamage;
        MaxSpeed = Stats.BaseSpeed;
        MaxMana = Stats.BaseMana;
        KritChance = Stats.BaseKritChance;
        KritModifier = Stats.BaseKritModifier;
        Hp = MaxHp;
        Armor = MaxArmor;
        Speed = MaxSpeed;
        Mana = MaxMana;
        EmitSignal(SignalName.StatsLoaded);
    }

    public virtual void TakeDamage(HitData data)
    {
        var damage = data.Damage;
        if (Armor <= 0) {
            HittedElement = ElementShaker.Shake(HittedElement, data.Element);
            damage *= Elements.GetElement(data.Element).DamageModifier;
            damage *= WeaponTypeNums.GetModifier(UnitWeapon, data.Element);
            Hp -= damage;
            if (Hp <= 0) ExecuteDie();
        }else {
            Armor -= damage;
        }
        EmitSignalDamageTaken(damage,  (int)data.Element);
    }

    public virtual void ExecuteDie()
    {
        QueueFree();
    }

    public virtual void AddSoulStats(Soul soul)
    {
        Hp = Stats.BaseHp + soul.SoulStats.BaseHp;
        MaxArmor = Stats.BaseArmor + soul.SoulStats.BaseArmor;
        Damage = Stats.BaseDamage + soul.SoulStats.BaseDamage;
        Speed = Stats.BaseSpeed + soul.SoulStats.BaseSpeed;
        Mana = Stats.BaseMana + soul.SoulStats.BaseMana;
        KritChance = Stats.BaseKritChance + soul.SoulStats.BaseKritChance;
        KritModifier = Stats.BaseKritModifier + soul.SoulStats.BaseKritModifier; 
    }
}