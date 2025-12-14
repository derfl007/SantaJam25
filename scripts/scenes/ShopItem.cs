using Godot;
using SantaJam25.scripts.resources;

namespace SantaJam25.scripts.scenes;

[Tool]
public partial class ShopItem : HBoxContainer
{
    [Export]
    public PowerUp PowerUp { get; private set; }

    public CheckBox CheckBox { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (PowerUp is null) return;
        GetNode<RichTextLabel>("NameLabel").Text = PowerUp.Name;
        GetNode<RichTextLabel>("CostLabel").Text = $"[color=gold]{PowerUp.Cost} Gold";
        CheckBox = GetNode<CheckBox>("CheckBox");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}