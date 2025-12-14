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
    ///     The maximum sales that this component can have before the nature damage reaches 100%
    /// </summary>
    [Export]
    public float MaxLifetimeSales { get; set; }

    [Export]
    public float BaseCost { get; set; }

    [Export]
    public ComponentStats ComponentStats { get; set; }
}