using System.Linq;
using Godot;
using SantaJam25.scripts.autoload;

namespace SantaJam25.scripts.levels;

public partial class MainMenu : Control
{
    private Button _startButton;
    private Button _optionsButton;
    private Button _quitButton;

    private VBoxContainer _buttonContainer;
    private VBoxContainer _savesContainer;
    private Button _savesBackButton;

    private Settings _settings;
    private ConfirmationDialog _confirmationDialog;
    private int _saveToDelete = -1;


    [Export]
    private PackedScene _worldMapScene;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _startButton = GetNode<Button>("%StartButton");
        _optionsButton = GetNode<Button>("%OptionsButton");
        _quitButton = GetNode<Button>("%QuitButton");

        _buttonContainer = GetNode<VBoxContainer>("ButtonContainer");
        _savesContainer = GetNode<VBoxContainer>("SavesContainer");
        _savesBackButton = GetNode<Button>("%SavesBackButton");

        _confirmationDialog = GetNode<ConfirmationDialog>("%ConfirmationDialog");

        _settings = GetNode<Settings>("%Settings");

        _settings.OnSave += () =>
        {
            _settings.Hide();
            _buttonContainer.Show();
        };

        _savesBackButton.Pressed += () =>
        {
            _buttonContainer.Show();
            _savesContainer.Hide();
        };

        _confirmationDialog.Confirmed += () =>
        {
            if (_saveToDelete == -1) return;
            GlobalGameState.Instance.DeleteSave(_saveToDelete);
            _saveToDelete = -1;
        };

        var nodes = _savesContainer.GetChildren();
        for (var index = 0; index < nodes.Count; index++)
        {
            var child = nodes[index];
            if (child is Button) continue;
            var loadButton = child.GetNode<Button>("LoadButton");
            var deleteButton = child.GetNode<Button>("DeleteButton");
            var lIndex = index;
            loadButton.Pressed += () =>
            {
                GlobalGameState.Instance.LoadSave(lIndex);
                GetTree().ChangeSceneToFile("res://levels/world_map.tscn");
            };
            deleteButton.Pressed += () => { ShowConfirmation(lIndex); };
        }

        _startButton.Pressed += () =>
        {
            _buttonContainer.Hide();
            _savesContainer.Show();
        };


        _optionsButton.Pressed += () =>
        {
            _settings.Show();
            _buttonContainer.Hide();
        };
        _quitButton.Pressed += () => { GetTree().Quit(); };
    }

    private void ShowConfirmation(int index)
    {
        _saveToDelete = index;
        _confirmationDialog.Show();
    }
}