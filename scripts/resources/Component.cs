using System;
using Godot;

namespace SantaJam25.scripts.resources;

[GlobalClass]
public partial class Component : Resource
{
    [Export]
    public string Name { get; set; }

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; }

    [Export]
    public Texture2D Texture { get; set; }

    [Export]
    public Color Color { get; set; }

    [Export]
    public ComponentType ComponentType { get; set; }

    [Export]
    public Rarity Rarity { get; set; }

    [Export]
    public Cost Cost { get; set; }

    [Export]
    public ComponentStats ComponentStats { get; set; }

    public int GetMaxLifeTimeSales()
    {
        return Rarity switch
        {
            Rarity.Abundant => 200,
            Rarity.Common => 100,
            Rarity.Rare => 75,
            Rarity.Infinite => 999999,
            _ => 0
        };
    }

    public int GetCost()
    {
        return Cost switch
        {
            Cost.Cheap => 10,
            Cost.Normal => 25,
            Cost.Expensive => 75,
            _ => 0
        };
    }
}

public enum Rarity
{
    Abundant,
    Common,
    Rare,
    Infinite
}

public enum Cost
{
    Cheap,
    Normal,
    Expensive
}