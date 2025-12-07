using Godot;

namespace SantaJam25.scripts.resources;

/// <summary>
///     Includes stats that can change dynamically after each round.
///     These will also be included in the save file
/// </summary>
[GlobalClass]
public partial class ComponentStats : Resource
{
    [Export(PropertyHint.Range, "0,100")]
    public int Cost { get; set; }

    [Export(PropertyHint.Range, "0,100")]
    public int Demand { get; set; }

    [Export(PropertyHint.Range, "0,100")]
    public int NatureDebt { get; set; }
}