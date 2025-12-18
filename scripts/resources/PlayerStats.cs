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
    public int Money = 300;

    [Export]
    public string Name { get; set; }

    [Export]
    public Texture2D PlayerTexture;
}