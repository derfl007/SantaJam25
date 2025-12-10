using Godot;
using Godot.Collections;

namespace SantaJam25.scripts.resources;

public partial class SaveState : Resource
{
    [Export]
    public int NatureDamage { get; set; }

    [Export]
    public Array<string> UnlockedLevels { get; set; } = ["event_1"];

    /// <summary>
    ///     Maps the current ComponentStats (cost, demand, nature-debt) to the component names
    /// </summary>
    [Export]
    public Dictionary<string, ComponentStats> CurrentComponentStats { get; set; } = new();

    [Export]
    public PlayerStats PlayerStats { get; set; } = new() { IsPlayer = true, Name = "Player" };

    [Export]
    public PlayerStats Enemy1Stats { get; set; } = new() { Name = "AI 1" };

    [Export]
    public PlayerStats Enemy2Stats { get; set; } = new() { Name = "AI 2" };

    [Export]
    public PlayerStats Enemy3Stats { get; set; } = new() { Name = "AI 3" };
}