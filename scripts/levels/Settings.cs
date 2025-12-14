using Godot;
using System;

namespace SantaJam25.scripts.levels;

public partial class Settings : Control
{
    private OptionButton _windowModeButton;
    private CheckButton _vSyncModeButton;
    private CheckButton _pixelPerfectButton;
    private Button _saveButton;

    [Signal]
    public delegate void OnSaveEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _windowModeButton = GetNode<OptionButton>("%WindowModeButton");
        _vSyncModeButton = GetNode<CheckButton>("%VSyncModeButton");
        _pixelPerfectButton = GetNode<CheckButton>("%PixelPerfectButton");

        _saveButton = GetNode<Button>("%SaveButton");

        _windowModeButton.Selected = (int)DisplayServer.WindowGetMode();

        _vSyncModeButton.ButtonPressed = (int)DisplayServer.WindowGetVsyncMode() > 0;

        _pixelPerfectButton.ButtonPressed =
            GetTree().Root.ContentScaleStretch == Window.ContentScaleStretchEnum.Integer;

        _windowModeButton.ItemSelected += index =>
        {
            DisplayServer.WindowSetMode((DisplayServer.WindowMode)index);
            DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, index > 0);
        };

        _vSyncModeButton.Pressed += () =>
        {
            DisplayServer.WindowSetVsyncMode(_vSyncModeButton.ButtonPressed
                ? DisplayServer.VSyncMode.Enabled
                : DisplayServer.VSyncMode.Disabled);
        };

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