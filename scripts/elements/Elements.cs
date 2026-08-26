using System;
using Godot;

namespace ProjectD.scripts.units;

public static class Elements
{
    public static Element
        Fire = new(1.2f, Colors.DarkRed, new Color("#FF7700"),new Color("#FFBB00") ),
        Water = new(1.15f, Colors.DodgerBlue, Colors.CornflowerBlue, Colors.DeepSkyBlue),
        Metallic = new(1.4f, new Color(0.5f, 0.5f, 0.5f),Colors.DarkGray, new Color(0.8f, 0.8f, 0.8f)),
        Air = new(1.2f, Colors.Bisque,  new Color(0.8f, 0.8f, 0.8f),  new Color(0.9f, 0.9f, 0.9f)),
        Darkness = new(1.2f, Colors.MidnightBlue, new Color(), new Color()),
        Holy = new(1.2f, Colors.Coral,  new Color(), new Color()),
        Nature = new(1.2f, Colors.DarkOliveGreen,  new Color(), new Color()),
        Ancient = new(1.5f, Colors.SlateBlue,  new Color(), new Color()),
        Explosive, 
        Steam = new(1.2f, Colors.DimGray,  new Color(), new Color()), 
        Melting, 
        Eternal, 
        Rusty,
        Blizzard, 
        Divine, 
        Overgrowth
        ;

    public static Element GetElement(ElementType e)
    {
        return e switch
        {
            ElementType.Fire => Fire,
            ElementType.Water => Water,
            ElementType.Metallic => Metallic,
            ElementType.Air => Air,
            ElementType.Darkness => Darkness,
            ElementType.Holy => Holy,
            ElementType.Nature => Nature,
            ElementType.Ancient => Ancient,
            ElementType.Explosive => Explosive,
            ElementType.Steam => Steam,
            ElementType.Melting => Melting,
            ElementType.Eternal => Eternal,
            ElementType.Rusty => Rusty,
            ElementType.Blizzard => Blizzard,
            ElementType.Divine => Divine,
            ElementType.Overgrowth => Overgrowth,
            _ => throw new ArgumentOutOfRangeException(nameof(e) + " on Elements", e, null)
        };
    }
}