using Godot;
using ProjectD.scripts.units;

[GlobalClass]
public partial class BulletResource : Resource
{
    [Export] public Texture2D[] Texture;
    [Export] public float AnimationSpeed;
    [Export] public float Damage;
    [Export] public float Spread;
    [Export] public float Speed;
    [Export] public float Lifetime;
    [Export] public float PierceCount = 1;
    [Export] public ElementType ElementType;
    [Export] public float CollisionRadius;
    
}