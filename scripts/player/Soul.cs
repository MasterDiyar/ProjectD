using System.Linq;
using Godot;
using ProjectD.scripts.units;

[GlobalClass]
public partial class Soul : Resource
{
    [Export] public ElementType SoulElement;
    [Export] public UnitStats   SoulStats;
    [Export] public Texture2D   SoulTexture, UnitTexture;
    [Export] public string      SoulName,
                                SoulDescription;

    /// <summary>
    /// <para>DirectImpact is direct stat adder for positional souls.
    /// DirectionalImpacts works to souls that higher or lower than current pointer
    /// </para>
    /// </summary>
    [Export] public int[] DirectImpact, DirectionalImpact;

    public int Position=-1;

    public virtual float ModifyDamage(Unit unit)
    {
        return unit.Damage;
    }
    
    public virtual float ModifyDamage(float damage)
    {
        return SoulStats.BaseDamage + damage;
    }

    public virtual void DirectImpactFunc(Unit unit)
    {
        
    }

    public virtual void DirectionalImpactFunc(Unit unit)
    {
        
    }

    public virtual void SoulEquipped(Unit unit, int position)
    {
        
    }

    public static Soul Clone(Soul soul) => new()
    {
        SoulElement =  soul.SoulElement,
        SoulName = soul.SoulName,
        SoulDescription = soul.SoulDescription,
        DirectImpact = soul.DirectImpact,
        DirectionalImpact = soul.DirectionalImpact,
        SoulTexture =  soul.SoulTexture,
        UnitTexture =  soul.UnitTexture,
        SoulStats = UnitStats.Clone(soul.SoulStats)
    };

    public bool InDirectImpact(int pos)
    {
        return DirectImpact.Any(num => num == pos);
    }

    public bool InDirectionalImpact(int pos)
    {
        return DirectImpact.Any(num => Position + num == pos);
    }
}

