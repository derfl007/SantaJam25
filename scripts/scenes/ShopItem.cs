using Godot;
using SantaJam25.scripts.resources;

namespace SantaJam25.scripts.scenes;

public partial class ShopItem : HBoxContainer
{
    [Export]
    public PowerUp PowerUp { get; private set; }

    public CheckBox CheckBox { get; private set; }

    public override void _Ready()
    {
        if (PowerUp is null) return;
        GetNode<RichTextLabel>("NameLabel").Text = PowerUp.Name;
        GetNode<RichTextLabel>("CostLabel").Text = $"[color=gold]{PowerUp.Cost} Gold";
        CheckBox = GetNode<CheckBox>("CheckBox");
    }
}