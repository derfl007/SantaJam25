using Godot;
using System;

namespace SantaJam25.scripts.levels;

public partial class Settings : Control
{
    private OptionButton _windowModeButton;
    private CheckButton _pixelPerfectButton;
    private Button _saveButton;

    [Signal]
    public delegate void OnSaveEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _windowModeButton = GetNode<OptionButton>("%WindowModeButton");
        _pixelPerfectButton = GetNode<CheckButton>("%PixelPerfectButton");

        _saveButton = GetNode<Button>("%SaveButton");

        _windowModeButton.Selected = DisplayServer.WindowGetMode() switch
        {
            DisplayServer.WindowMode.Fullscreen => // fullscreen
                0,
            DisplayServer.WindowMode.Maximized => // borderless
                1,
            DisplayServer.WindowMode.Windowed => //windowed
                2,
            _ => _windowModeButton.Selected
        };

        _pixelPerfectButton.ButtonPressed =
            GetTree().Root.ContentScaleStretch == Window.ContentScaleStretchEnum.Integer;

        _windowModeButton.ItemSelected += (index =>
        {
            switch (index)
            {
                case 0: // fullscreen
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.ExclusiveFullscreen);
                    DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
                    break;
                case 1: // borderless
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Maximized);
                    DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, true);
                    break;
                case 2: //windowed
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                    DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, false);
                    break;
            }
        });

        _pixelPerfectButton.Pressed += () =>
        {
            GetTree().Root.ContentScaleStretch = _pixelPerfectButton.ButtonPressed
                ? Window.ContentScaleStretchEnum.Integer
                : Window.ContentScaleStretchEnum.Fractional;
        };

        _saveButton.Pressed += EmitSignalOnSave;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}