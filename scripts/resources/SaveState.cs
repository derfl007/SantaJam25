using Godot;
using Godot.Collections;

namespace SantaJam25.scripts.resources;

public partial class SaveState : Resource
{
    [Export]
    public int Gold { get; set; }

    [Export]
    public int NatureDamage { get; set; }

    [Export]
    public Array<string> UnlockedLevels { get; set; } = ["event_1"];

    /// <summary>
    ///     Maps the current ComponentStats (cost, demand, nature-debt) to the component names
    /// </summary>
    [Export]
    public Dictionary<string, ComponentStats> CurrentComponentStats { get; set; } = new();
}