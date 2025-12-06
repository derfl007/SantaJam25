using Godot;
using SantaJam25.scripts.resources;

namespace SantaJam25.scripts.autoload;

public partial class GlobalGameState : Node
{
    private const string SavePath = "user://savegame.tres";

    [Export]
    public SaveState CurrentSave { get; private set; } = new();

    public override void _Ready()
    {
    }

    public void LoadGame()
    {
        GD.Print("Loading game...");
        if (!FileAccess.FileExists(SavePath))
        {
            GD.Print("Save file not found, creating...");
            SaveGame();
            return;
        }

        CurrentSave = ResourceLoader.Load<SaveState>(SavePath);

        GD.Print("Game loaded");
    }

    public void SaveGame()
    {
        GD.Print("Saving game...");
        ResourceSaver.Save(CurrentSave, SavePath);
    }

    public override void _ExitTree()
    {
        SaveGame();
    }
}