using System.Collections.Generic;
using Godot;
using ProjectD.scripts.player;
using ProjectD.scripts.weapon;

namespace ProjectD.scripts.units;

public partial class Unit : CharacterBody2D, IHasUnit
{
    [Signal] public delegate void HealthChangedEventHandler(float currentHp, float maxHp);
    [Signal] public delegate void DamageTakenEventHandler(float amount, int elementType);
    [Signal] public delegate void HealedEventHandler(float amount);
    [Signal] public delegate void StatsLoadedEventHandler();
    [Export] public UnitStats Stats;
    [Export] public Weapon Weapon;
    [Export] public Soul CurrentSoul;
    public float Hp, MaxHp;
    public float Armor, MaxArmor;
    public float Damage;
    public float Speed, MaxSpeed;
    public float Mana, MaxMana;
    public float KritChance;
    public float KritModifier;
    
    public List<Soul> _souls = []; 
    public List<WeaponResource> _weapons = []; 

    public int CurrentSoulIndex = 0;
    public int CurrentWeaponIndex = 0;
    
    public Unit unit { get => this; set{ if (value==null) return;}}

    public ElementType UnitElement => CurrentSoul?.SoulElement ?? ElementType.Fire;
    public ElementType HittedElement;
    public WeaponType UnitWeapon;

    public override void _Ready()
    {
        Stats = UnitStats.Clone(Stats);
        if (CurrentSoul != null) {
            CurrentSoul = Soul.Clone(CurrentSoul);
            AddSoulStats(CurrentSoul);
        }
        
        SetDefaultStats();
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
            Game.Instance.Pool.CreateTicket(GlobalPosition, BloodPool.BloodRect, data.AttackAngle);
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

    public virtual void SetDefaultStats()
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
    }

    public virtual void AddSoulStats(Soul soul)
    {
        MaxHp = Stats.BaseHp + soul.SoulStats.BaseHp;
        MaxArmor = Stats.BaseArmor + soul.SoulStats.BaseArmor;
        Damage = Stats.BaseDamage + soul.SoulStats.BaseDamage;
        MaxSpeed = Stats.BaseSpeed + soul.SoulStats.BaseSpeed;
        MaxMana = Stats.BaseMana + soul.SoulStats.BaseMana;
        KritChance = Stats.BaseKritChance + soul.SoulStats.BaseKritChance;
        KritModifier = Stats.BaseKritModifier + soul.SoulStats.BaseKritModifier; 
        
        Speed =  MaxSpeed;
        Mana = Mathf.Min(Mana + soul.SoulStats.BaseMana, MaxMana);
        Hp = Mathf.Min(Hp, MaxHp);
        Armor = Mathf.Min(Armor + soul.SoulStats.BaseArmor, MaxArmor);
        CurrentSoul.SoulElement = soul.SoulElement;
        CurrentSoul = soul;
    }

    public void MultiSoulAdder(Soul[] soul)
    {
        UnitStats stats = UnitStats.Zero;
        foreach (var s in soul)
            if (s?.SoulStats != null)
                stats.Add(s.SoulStats);
        
        CurrentSoul.SoulStats = stats;
    }
}