using Godot;

namespace SantaJam25.scripts.resources;

[GlobalClass]
public partial class PlayerStats : Resource
{
    [Export]
    public Weapon CurrentWeapon;

    [Export]
    public bool IsPlayer;

    [Export]
    public int Money;

    [Export]
    public string Name { get; set; }
}