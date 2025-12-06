using Godot;

namespace SantaJam25.scripts.levels;

public partial class FactoryNodeOverlay : NodeOverlay
{
    public override void _UnhandledInput(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventKey { Pressed: true, Keycode: Key.Escape }:
                EmitSignalCloseNodeOverlay(false);
                GetViewport().SetInputAsHandled();
                break;
            case InputEventKey { Pressed: true, Keycode: Key.E } eventKey:
                EmitSignalCloseNodeOverlay(true);
                GetViewport().SetInputAsHandled();
                break;
        }
    }
}