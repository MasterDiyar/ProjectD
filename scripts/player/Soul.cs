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

    [Export] public int[] DirectImpact, DirectionalImpact;

    public virtual float ModifyDamage(Unit unit)
    {
        return unit.Damage;
    }
    
    //Poka ne pridumal kak modifiiit i ostavlu pustim void void void 
}

