using Godot;
using SantaJam25.scripts.autoload;

namespace SantaJam25.scripts.levels;

public partial class PauseMenu : CanvasLayer
{
    private Button _continueButton;
    private Button _optionsButton;
    private Button _mainMenuButton;
    private Button _quitGameButton;

    private VBoxContainer _buttonContainer;

    private Settings _settings;

    public override void _Ready()
    {
        _continueButton = GetNode<Button>("%ContinueButton");
        _optionsButton = GetNode<Button>("%OptionsButton");
        _mainMenuButton = GetNode<Button>("%MainMenuButton");
        _quitGameButton = GetNode<Button>("%QuitGameButton");

        _buttonContainer = GetNode<VBoxContainer>("%ButtonContainer");

        _settings = GetNode<Settings>("%Settings");

        _continueButton.Pressed += () =>
        {
            Visible = false;
            GetTree().Paused = false;
        };
        _optionsButton.Pressed += ShowSettings;
        _mainMenuButton.Pressed += () =>
        {
            GlobalGameState.Instance.SaveGame();
            GetTree().ChangeSceneToFile("res://levels/main_menu.tscn");
        };
        _quitGameButton.Pressed += () =>
        {
            GlobalGameState.Instance.SaveGame();
            GetTree().Quit();
        };
        _settings.OnSave += HideSettings;
    }

    private void ShowSettings()
    {
        _buttonContainer.Visible = false;
        _settings.Visible = true;
    }

    private void HideSettings()
    {
        _settings.Visible = false;
        _buttonContainer.Visible = true;
    }
}