using Godot;

namespace SantaJam25.scripts.resources;

[GlobalClass]
public partial class PowerUp : Resource
{
    public enum PowerUpType
    {
        RevealHighDemand,
        RevealLowDemand,
        RevealHighNatureDebt,
        RevealLowNatureDamage,
        SabotageQuality
    }

    [Export]
    public string Name;

    [Export]
    public PowerUpType Type;

    [Export]
    public int Usages;

    [Export]
    public int Cost;
}