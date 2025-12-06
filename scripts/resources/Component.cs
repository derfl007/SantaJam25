using Godot;

namespace SantaJam25.scripts.resources;

[GlobalClass]
public partial class Component : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public string Description { get; set; }

    [Export]
    public Texture Texture { get; set; }

    [Export]
    public ComponentType ComponentType { get; set; }

    [ExportGroup("Stats"), Export(PropertyHint.Range, "0,100")]
    public int Cost { get; set; }

    [Export(PropertyHint.Range, "0,100")]
    public int Demand { get; set; }

    [Export(PropertyHint.Range, "0,100")]
    public int NatureDebt { get; set; }
}