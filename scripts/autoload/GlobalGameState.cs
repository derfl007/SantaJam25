using Godot;
using SantaJam25.scripts.resources;

namespace SantaJam25.scripts.autoload;

public partial class GlobalGameState : Node
{
    public static GlobalGameState Instance { get; private set; }

    [Signal]
    public delegate void SaveUpdateEventHandler();

    public string SavePath;
    private const string TemplatePath = "res://resources/global/savegame.tres";

    [Export]
    public SaveState CurrentSave { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }

    public void LoadGame()
    {
        if (SavePath.Length == 0) return;
        GD.Print("Loading game...");
        if (!FileAccess.FileExists(SavePath))
        {
            GD.Print("Save file not found, creating...");
            var templateSave = ResourceLoader.Load<SaveState>(TemplatePath);
            ResourceSaver.Save(templateSave, SavePath);
        }

        CurrentSave = ResourceLoader.Load<SaveState>(SavePath);
        EmitSignalSaveUpdate();

        GD.Print("Game loaded");
    }

    public void SaveGame()
    {
        if (SavePath.Length == 0) return;
        GD.Print("Saving game...");
        ResourceSaver.Save(CurrentSave, SavePath);
        EmitSignalSaveUpdate();
        GD.Print("Game saved");
    }

    public void LoadSave(int index)
    {
        SavePath = $"user://savegame_{index}.tres";
        LoadGame();
    }

    public void DeleteSave(int index)
    {
        var dirAccess = DirAccess.Open("user://");
        dirAccess.Remove($"user://savegame_{index}.tres");
    }

    public override void _ExitTree()
    {
        SaveGame();
    }
}