using Godot;

namespace SantaJam25.scripts.scenes;

public partial class WorldMapPlayer : CharacterBody2D
{
    [Export]
    private float _movementSpeed = 25.0f;

    public NinePatchRect NinePatchRect { get; private set; }

    public NavigationAgent2D NavAgent;

    private Sprite2D _sprite;

    public bool IsMoving { get; set; }

    public override void _Ready()
    {
        NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        NinePatchRect = GetNode<NinePatchRect>("NinePatchRect");
        _sprite = GetNode<Sprite2D>("Sprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (NavAgent.IsNavigationFinished()) return;

        var currentAgentPosition = GlobalTransform.Origin;
        var nextPathPosition = NavAgent.GetNextPathPosition();

        Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * _movementSpeed;

        _sprite.FlipH = Velocity.X < 0;

        MoveAndSlide();
    }

    public void SetTargetPosition(Vector2 position)
    {
        NavAgent.SetTargetPosition(position);
    }
}