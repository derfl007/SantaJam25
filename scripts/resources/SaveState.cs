using Godot;
using Godot.Collections;

namespace SantaJam25.scripts.resources;

[GlobalClass]
public partial class SaveState : Resource
{
    [Export]
    public int NatureDamage { get; set; }

    [Export]
    public Array<string> UnlockedLevels { get; set; } = ["event_1"];

    [Export]
    public Array<string> CompletedLevels { get; set; } = [];

    /// <summary>
    ///     Maps the current ComponentStats (cost, demand, nature-debt) to the component names
    /// </summary>
    [Export]
    public Dictionary<string, ComponentStats> CurrentComponentStats { get; set; } = new();

    [Export]
    public Array<PowerUp> PowerUps { get; set; } = [];

    [Export]
    public PlayerStats PlayerStats { get; set; } = new() { IsPlayer = true, Name = "Player" };

    [Export]
    public PlayerStats Enemy1Stats { get; set; } = new() { Name = "AI 1" };

    [Export]
    public PlayerStats Enemy2Stats { get; set; } = new() { Name = "AI 2" };

    [Export]
    public PlayerStats Enemy3Stats { get; set; } = new() { Name = "AI 3" };

    [Export]
    public Vector2 PlayerPosition { get; set; } = new(24, 24);
}