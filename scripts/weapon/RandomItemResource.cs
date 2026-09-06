
using Godot;

[GlobalClass]
public partial class RandomItemResource : Resource
{
    [Export] WeaponResource[] CommonWeapons, RareWeapons, EpicWeapons, LegendaryWeapons;

    public WeaponResource GetWeapon(float chance, float level)
    {
        level = Mathf.Max(level, 1f);
        var levelModifier = 1/level;
        if (chance >= 0 && chance < 0.5f * levelModifier) return GetRandomWeaponFromArray(CommonWeapons);
        if (chance >= 0.5f * levelModifier && chance < 0.85f * levelModifier) return GetRandomWeaponFromArray(RareWeapons);
        if (chance >= 0.85f * levelModifier && chance < 0.95f * levelModifier) return GetRandomWeaponFromArray(EpicWeapons);
        return GetRandomWeaponFromArray(LegendaryWeapons);
    }

    private WeaponResource GetRandomWeaponFromArray(WeaponResource[] weapons) => weapons[GD.Randi()%weapons.Length];
}