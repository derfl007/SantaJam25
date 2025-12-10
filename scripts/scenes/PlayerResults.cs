using Godot;
using System;

public partial class PlayerResults : VBoxContainer
{
    private Label _goldLabel;
    private Label _salesLabel;
    private TextureProgressBar _shareProgressBar;
    private Label _nameLabel;

    public override void _Ready()
    {
        _goldLabel = GetNode<Label>("GoldLabel");
        _salesLabel = GetNode<Label>("SalesLabel");
        _shareProgressBar = GetNode<TextureProgressBar>("ShareProgressBar");
        _nameLabel = GetNode<Label>("NameLabel");
    }

    public void UpdateValues((int gold, int sales, int share, string playerName) values)
    {
        _goldLabel.Text = $"{values.gold} Gold";
        _salesLabel.Text = $"{values.sales} Sales";
        _shareProgressBar.Value = values.share;
        _nameLabel.Text = values.playerName;
    }
}