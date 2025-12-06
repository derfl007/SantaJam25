using Godot;

namespace SantaJam25.scripts.levels;

public partial class ShopNodeOverlay : NodeOverlay
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is not InputEventKey { Pressed: true, Keycode: Key.Escape }) return;
        EmitSignalCloseNodeOverlay(true);
        GetViewport().SetInputAsHandled();
    }
}