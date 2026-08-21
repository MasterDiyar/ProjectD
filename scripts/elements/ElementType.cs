namespace ProjectD.scripts.units;

public enum ElementType
{
    //bricks
    Fire,
    Water,
    Metallic,
    Air,
    Darkness,
    Holy,
    Nature,
    Ancient,
    //combinations
    Explosive, // fire + darkness
    Steam, // fire + water 
    Melting, // fire + metallic
    Eternal, // fire + ancient
    Rusty, // water + metallic
    Blizzard, // water + darkness
    Divine, //water + holy
    Overgrowth, //water + nature
}

public static class ElementShaker
{
    public static ElementType Shake(ElementType left, ElementType right)
    {
        return (left, right) switch
        {
            (ElementType.Fire, ElementType.Darkness) or (ElementType.Darkness, ElementType.Fire) => ElementType.Explosive,
            (ElementType.Fire, ElementType.Water) or (ElementType.Water, ElementType.Fire) => ElementType.Steam,
            (ElementType.Fire, ElementType.Metallic) or (ElementType.Metallic, ElementType.Fire) => ElementType.Melting,
            (ElementType.Fire, ElementType.Ancient) or (ElementType.Ancient, ElementType.Fire) => ElementType.Eternal,
            
            (ElementType.Water, ElementType.Metallic) or (ElementType.Metallic, ElementType.Water) => ElementType.Rusty,
            (ElementType.Water, ElementType.Darkness) or (ElementType.Darkness, ElementType.Water) => ElementType.Blizzard,
            (ElementType.Water, ElementType.Holy) or (ElementType.Holy, ElementType.Water) => ElementType.Divine,
            (ElementType.Water, ElementType.Nature) or (ElementType.Nature, ElementType.Water) => ElementType.Overgrowth,
            
            // Если ни один паттерн не совпал, возвращаем правый элемент (right)
            _ => right 
        };
    }
}

