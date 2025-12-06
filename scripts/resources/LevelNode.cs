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

    [Export]
    public Array<LevelNode> NextNodes;

    [Export]
    public Vector2I TileMapCoords;

    [Export]
    public LevelNodeType Type;

    public Vector2I GetAtlasCoords(bool isUnlocked)
    {
        return Type switch
        {
            LevelNodeType.Event when !isUnlocked => new Vector2I(50, 30),
            LevelNodeType.Event => new Vector2I(51, 30),
            LevelNodeType.Factory when !isUnlocked => new Vector2I(52, 30),
            LevelNodeType.Factory => new Vector2I(53, 30),
            LevelNodeType.Shop when !isUnlocked => new Vector2I(54, 30),
            LevelNodeType.Shop => new Vector2I(55, 30),
            _ => new Vector2I(-1, -1)
        };
    }
}