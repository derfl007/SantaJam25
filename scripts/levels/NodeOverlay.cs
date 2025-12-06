using Godot;

namespace SantaJam25.scripts.levels;

public partial class NodeOverlay : Control
{
    [Signal]
    public delegate void CloseNodeOverlayEventHandler(bool success);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}