using Godot;
using SantaJam25.scripts.autoload;
using SantaJam25.scripts.resources;
using SantaJam25.scripts.util;

namespace SantaJam25.scripts.scenes;

public partial class Weapon : Control
{
    private Component _blade;
    private TextureRect _bladeTextureRect;

    private GlobalGameState _globalGameState;
    private Component _hilt;

    private TextureRect _hiltTextureRect;
    private Component _quench;

    private GpuParticles2D _quenchParticles;

    [Export]
    public Component Blade
    {
        get
        {
            if (_blade is null) return null;
            _blade.ComponentStats =
                _globalGameState.CurrentSave.CurrentComponentStats[_blade.Name] ?? _blade.ComponentStats;
            return _blade;
        }
        set
        {
            _blade = value;
            if (_blade is null) return;
            _globalGameState.CurrentSave.CurrentComponentStats[_blade.Name] = _blade.ComponentStats;
            _bladeTextureRect.Texture = value.Texture;
        }
    }

    [Export]
    public Component Hilt
    {
        get
        {
            if (_hilt is null) return null;
            _hilt.ComponentStats =
                _globalGameState.CurrentSave.CurrentComponentStats[_hilt.Name] ?? _hilt.ComponentStats;
            return _hilt;
        }
        set
        {
            _hilt = value;
            if (_hilt is null) return;
            var currentComponentStats = _globalGameState.CurrentSave.CurrentComponentStats;
            currentComponentStats[_hilt.Name] = _hilt.ComponentStats;

            _hiltTextureRect.Texture = value.Texture;
        }
    }

    [Export]
    public Component Quench
    {
        get
        {
            if (_quench is null) return null;
            _quench.ComponentStats =
                _globalGameState.CurrentSave.CurrentComponentStats[_quench.Name] ?? _quench.ComponentStats;
            return _quench;
        }
        set
        {
            _quench = value;
            if (_quench is null) return;
            _globalGameState.CurrentSave.CurrentComponentStats[_quench.Name] = _quench.ComponentStats;
            _quenchParticles.Texture = value.Texture;
        }
    }

// Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _globalGameState = this.GetGlobalGameState();

        _hiltTextureRect = GetNode<TextureRect>("HiltTextureRect");
        _bladeTextureRect = GetNode<TextureRect>("BladeTextureRect");
        _quenchParticles = GetNode<GpuParticles2D>("QuenchParticleEmitter");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    /// <summary>
    ///     The total production cost of this weapon
    /// </summary>
    /// <returns></returns>
    public int GetTotalCost()
    {
        return Blade.ComponentStats.Cost + Hilt.ComponentStats.Cost + Quench.ComponentStats.Cost;
    }

    /// <summary>
    ///     The total price this weapon sells for
    /// </summary>
    /// <returns></returns>
    public int GetTotalPrice()
    {
        return Blade.ComponentStats.Demand + Hilt.ComponentStats.Demand + Quench.ComponentStats.Demand;
    }

    public void IncreaseComponentUsages()
    {
        Blade.IncreaseUsage();
        Hilt.IncreaseUsage();
        Quench.IncreaseUsage();
    }
}