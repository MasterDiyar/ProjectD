using Godot;
using ProjectD.scripts.player;
using ProjectD.scripts.weapon;

namespace ProjectD.scripts.units;

public partial class Unit : CharacterBody2D
{
    [Signal] public delegate void StatsLoadedEventHandler();
    [Export]public UnitStats Stats;
    public float Hp;
    public float Armor;
    public float Damage;
    public float Speed;
    public float Mana;
    public float KritChance;
    public float KritModifier;
    
    public ElementType UnitElement, HittedElement;
    public WeaponType UnitWeapon;

    public override void _Ready()
    {
        Hp = Stats.BaseHp;
        Armor = Stats.BaseArmor;
        Damage = Stats.BaseDamage;
        Speed = Stats.BaseSpeed;
        Mana = Stats.BaseMana;
        KritChance = Stats.BaseKritChance;
        KritModifier = Stats.BaseKritModifier;
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
    }

    public virtual void ExecuteDie()
    {
        
        QueueFree();
    }
}