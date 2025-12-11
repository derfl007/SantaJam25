using Godot;

namespace SantaJam25.scripts.scenes;

public partial class PlayerResults : Control
{
    private Label _goldLabel;
    private Label _salesLabel;
    private TextureProgressBar _shareProgressBar;
    private Label _nameLabel;

    public override void _Ready()
    {
        _goldLabel = GetNode<Label>("VBoxContainer/GoldLabel");
        _salesLabel = GetNode<Label>("VBoxContainer/SalesLabel");
        _shareProgressBar = GetNode<TextureProgressBar>("VBoxContainer/ShareProgressBar");
        _nameLabel = GetNode<Label>("VBoxContainer/NameLabel");
    }

    public void UpdateValues((int gold, int sales, int share, string playerName) values)
    {
        _goldLabel.Text = $"{values.gold} Gold";
        _salesLabel.Text = $"{values.sales} Sales";
        _shareProgressBar.Value = values.share;
        _nameLabel.Text = values.playerName;
    }
}