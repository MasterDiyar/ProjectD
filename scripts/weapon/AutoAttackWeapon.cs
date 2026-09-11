using Godot;
using ProjectD.scripts.units;

namespace ProjectD.scripts.weapon;

public partial class AutoAttackWeapon : Node2D, IHasUnit
{
    [Export] public WeaponResource Resource;
    [Export] public WeaponType Type;
    [Export] public Sprite2D SpriteSlot;
    [Export] public Unit unit {get; set;}
    private float _t;
    PackedScene _bulletScene;
    public override void _Ready()
    {
        base._Ready();
        if (GetParent() is IHasUnit a)
            unit = a.unit;
        if (Resource.Texture.Length>0)
            SpriteSlot.Texture = Resource.Texture[0];
        _t = -Resource.SpawnTimeOffset;
        _bulletScene = GD.Load<PackedScene>("res://scenes/bullet/bullet.tscn");
    }

    public override void _Process(double delta)
    {
        _t += (float)delta;
        if (_t <= Resource.AttackSpeed) return;
        _t = 0;
        
        
         for (int i = 0; i < Resource.BulletCount; i++)
         {
             Bullet bullet = _bulletScene.Instantiate<Bullet>();
             bullet.Rotation = GD.Randf() * Mathf.Tau;
             bullet.GlobalPosition = (Type == WeaponType.Melee? Vector2.Zero: unit.GlobalPosition) + Vector2.FromAngle(bullet.Rotation) * Resource.OffsetSpawn;
             bullet.unit = unit;
             bullet.WeaponType = Type;
             bullet += Resource.Bullet;
             bullet.GlobalScale = Vector2.One * Resource.BulletScale;

             if (Type == WeaponType.Melee) GetParent().AddChild(bullet);
             else Game.Instance.SpawnNode(bullet);
         }
    }
}