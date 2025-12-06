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
}