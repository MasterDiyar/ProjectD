using ProjectD.scripts.units;

namespace ProjectD.scripts.weapon;

public enum WeaponType { Melee, Ranged, Magic }

public static class WeaponTypeNums
{
    public static float GetModifier(WeaponType type, ElementType elementType)
    {
        return type switch
        {
            WeaponType.Melee => elementType switch
            {
                ElementType.Metallic => 1.9f,
                ElementType.Fire => 1.3f,
                ElementType.Ancient => 3,
                ElementType.Explosive => 2.3f,
                ElementType.Rusty => .9f,
                _ => 1
            },
            WeaponType.Magic => elementType switch
            {
                ElementType.Fire => 1.5f,
                ElementType.Ancient => 3f,
                ElementType.Air => 1.45f,
                ElementType.Water => 1.5f,
                ElementType.Blizzard => 1.9f,
                ElementType.Holy => 2f,
                ElementType.Nature => 1.1f,
                ElementType.Steam => 2.25f,
                _ => 1
            },
            WeaponType.Ranged => elementType switch
            {
                ElementType.Fire => 2f,
                ElementType.Water => .7f,
                ElementType.Steam => 1.44f,
                _ => 1
            }
        };
    }
}