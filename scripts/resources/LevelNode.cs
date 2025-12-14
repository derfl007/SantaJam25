using Godot;
using Godot.Collections;

namespace SantaJam25.scripts.resources;

[GlobalClass]
public partial class LevelNode : Resource
{
    public enum LevelNodeType
    {
        Event, // A global event that changes demands, prices, etc.
        Shop, // A shop tile where you can buy upgrades
        Factory // A factory tile where you have to design a new weapon
    }

    [Export]
    public string Name;

    [Export(PropertyHint.MultilineText)]
    public string EventDescription;

    [Export]
    public Array<LevelNode> NextNodes;

    [Export]
    public Vector2I TileMapCoords;

    [Export]
    public LevelNodeType Type;

    public Vector2I GetAtlasCoords(bool isUnlocked, bool isCompleted)
    {
        return Type switch
        {
            LevelNodeType.Event when !isUnlocked => new Vector2I(0, 0),
            LevelNodeType.Event when !isCompleted => new Vector2I(0, 1),
            LevelNodeType.Event => new Vector2I(0, 2),
            LevelNodeType.Factory when !isUnlocked => new Vector2I(1, 0),
            LevelNodeType.Factory when !isCompleted => new Vector2I(1, 1),
            LevelNodeType.Factory => new Vector2I(1, 2),
            LevelNodeType.Shop when !isUnlocked => new Vector2I(2, 0),
            LevelNodeType.Shop when !isCompleted => new Vector2I(2, 1),
            LevelNodeType.Shop => new Vector2I(2, 2),
            _ => new Vector2I(-1, -1)
        };
    }
}