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
}

public enum Rarity
{
    Abundant = 4000,
    Common = 1500,
    Rare = 500
}

public enum Cost
{
    Cheap = 10,
    Normal = 25,
    Expensive = 75
}