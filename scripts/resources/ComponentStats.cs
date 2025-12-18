using Godot;

namespace SantaJam25.scripts.resources;

/// <summary>
///     Includes stats that can change dynamically after each round.
///     These will also be included in the save file
/// </summary>
[GlobalClass]
public partial class ComponentStats : Resource
{
    [Export]
    public int Cost { get; set; } = -1;

    [Export(PropertyHint.Range, "1,10")]
    public float Demand { get; set; } = 5f;

    [Export(PropertyHint.Range, "0,100")]
    public int NatureDebt { get; set; }
}