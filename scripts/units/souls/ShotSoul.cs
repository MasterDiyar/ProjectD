
using System.Linq;
using Godot;
using ProjectD.scripts.player;
using ProjectD.scripts.units;

[GlobalClass]
public partial class ShotSoul : Soul
{
    [Export] public WeaponResource Resource;
    [Export] public int PerUse = 1;
    [Export] public PackedScene BulletScene;

    public override void DirectImpactFunc(Unit unit)
    {
        if (unit.CurrentSoulIndex != Position && InDirectImpact(unit.CurrentSoulIndex))
        {
            if (unit is not PlayerController player) return;

            player.Attack += Shoot;
        }
    }

    public void Shoot(float angle, PlayerController player)
    {
        //TODO weapon and rework weapon
    }
}