using Godot;
using SantaJam25.scripts.resources;

namespace SantaJam25.scripts.scenes;

public partial class WeaponResultScene : WeaponScene
{
    private RichTextLabel _qualityLabel;
    private RichTextLabel _nameLabel;
    private TextureRect _playerTextureRect;

    public PlayerStats Player { get; set; }

// Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        _qualityLabel = GetNode<RichTextLabel>("QualityLabel");
        _nameLabel = GetNode<RichTextLabel>("NameLabel");
        _playerTextureRect = GetNode<TextureRect>("PlayerTextureRect");
    }

    public override void UpdateTextures()
    {
        base.UpdateTextures();
        _qualityLabel.Text = "Quality: [color=" + Weapon.Quality switch
        {
            >= 1.2f => "cyan] Perfect",
            >= 1.0f => "green] Good",
            >= 0.8f => "orange] Okay",
            _ => "red] Bad",
        };
        _nameLabel.Text = Player?.Name;
        _playerTextureRect.Texture = Player?.PlayerTexture;
    }
}