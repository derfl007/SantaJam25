using Godot;

namespace SantaJam25.scripts.levels;

public partial class EventNodeOverlay : NodeOverlay
{
    public string Description { get; set; }

    public override void _Ready()
    {
        GetNode<RichTextLabel>("DescriptionLabel").Text = Description;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventKey { Pressed: true, Keycode: Key.E }:
                EmitSignalCloseNodeOverlay(true);
                GetViewport().SetInputAsHandled();
                break;
        }
    }
}