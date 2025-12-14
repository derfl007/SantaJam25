using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SantaJam25.scripts.autoload;

namespace SantaJam25.scripts.util;

public static class Utils
{
    /// <summary>
    ///     Just a shortcut for GetTree().Root.GetNode();
    /// </summary>
    public static T GetRootChild<T>(this Node node, NodePath path) where T : Node
    {
        return node.GetTree().Root.GetNode<T>(path);
    }

    public static GlobalGameState GetGlobalGameState(this Node node)
    {
        return node.GetRootChild<GlobalGameState>("/root/GlobalGameState");
    }

    public static Vector2 MapToGlobal(this TileMapLayer tileMapLayer, Vector2I mapCoords)
    {
        return tileMapLayer.ToGlobal(tileMapLayer.MapToLocal(mapCoords));
    }

    public static Vector2I GlobalToMap(this TileMapLayer tileMapLayer, Vector2 globalCoords)
    {
        return tileMapLayer.LocalToMap(tileMapLayer.ToLocal(globalCoords));
    }

    public static Array<T> ToGodotArray<[MustBeVariant] T>(this IEnumerable<T> enumerable)
    {
        return new Array<T>(enumerable);
    }
}