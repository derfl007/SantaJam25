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

    /// <summary>
    ///     Influences how much nature debt is acquired with each usage.
    ///     Is multiplied with the number of sales a weapon with this component gets in a round.
    /// </summary>
    [Export(PropertyHint.Range, "0,2")]
    public float NatureImpactFactor { get; set; }

    [Export]
    public float BaseCost { get; set; }

    [Export]
    public ComponentStats ComponentStats { get; set; }
}