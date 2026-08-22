
using Godot;

[GlobalClass]
public partial class UnitResource : Resource
{
    [Export] public Texture2D[] Textures;
    [Export] public Godot.Collections.Array<StatEnum> Stats;
    [Export] public float[] Add;
    
}