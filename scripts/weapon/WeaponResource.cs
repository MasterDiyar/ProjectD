
using Godot;
using ProjectD.scripts.weapon;

[GlobalClass]
public partial class WeaponResource : Resource
{
    [Export] public Texture2D[] Texture;
    [Export] public WeaponType Type;
    [Export] public int BulletCount;
    [Export] public BulletResource Bullet;
    [Export] public float BulletScale =1;
    [Export] public float AttackSpeed;
    [Export] public float BetweenAngle;
    [Export] public float OffsetAngle;
    [Export] public float OffsetSpawn;
    
    [ExportGroup("Animations")]
    [Export] public Animation AttackAnimation, IdleAnimation, UltimateAnimation;
}