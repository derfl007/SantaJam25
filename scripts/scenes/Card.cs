using Godot;
using SantaJam25.scripts.resources;

namespace SantaJam25.scripts.scenes;

public partial class Card : PathFollow2D
{
    private bool _disabled;
    private float _lerpProgress;
    public Sprite2D Sprite;

    [Export]
    public float AnimationSpeed = 10f;

    public Area2D Area2D;

    [Export]
    public Component Component;

    public float TargetProgress;
    public float TargetYPosition;

    public bool Disabled
    {
        get => _disabled;
        set
        {
            _disabled = value;
            Area2D.Monitoring = !_disabled;
            Sprite.Modulate = new Color(Colors.White, _disabled ? 0.5f : 1f);
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Sprite = GetNode<Sprite2D>("Sprite2D");
        Area2D = GetNode<Area2D>("Sprite2D/Area2D");

        if (Component == null) return;
        GetNode<Label>("%TitleLabel").Text = Component.Name;
        GetNode<TextureRect>("%TextureRect").Texture = Component.Texture;
        GetNode<Label>("%DescriptionLabel").Text = Component.Description;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Progress = Mathf.Lerp(Progress, TargetProgress, (float)delta * AnimationSpeed);
        Sprite.Position =
            Sprite.Position.Lerp(Sprite.Position with { Y = TargetYPosition }, (float)delta * AnimationSpeed);
    }
}