using Godot;
using SantaJam25.scripts.autoload;
using SantaJam25.scripts.resources;
using SantaJam25.scripts.util;

namespace SantaJam25.scripts.scenes;

public partial class WeaponScene : Control
{
    private Component _blade;
    private TextureRect _bladeTextureRect;

    private GlobalGameState _globalGameState;
    private Component _hilt;

    private TextureRect _hiltTextureRect;
    private Component _quench;


    public Weapon Weapon { get; set; } = new();

// Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _globalGameState = this.GetGlobalGameState();

        _hiltTextureRect = GetNode<TextureRect>("HiltTextureRect");
        _bladeTextureRect = GetNode<TextureRect>("BladeTextureRect");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void UpdateTextures()
    {
        _bladeTextureRect.Texture = Weapon.Blade?.Texture;
        _hiltTextureRect.Texture = Weapon.Hilt?.Texture;
        _bladeTextureRect.SetInstanceShaderParameter("line_color",
            Weapon.Quench?.Color ?? Colors.White);
    }
}