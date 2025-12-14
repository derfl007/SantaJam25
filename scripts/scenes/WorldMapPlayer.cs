using Godot;

namespace SantaJam25.scripts.scenes;

public partial class WorldMapPlayer : CharacterBody2D
{
    [Export]
    private float _movementSpeed = 25.0f;

    public NinePatchRect NinePatchRect { get; private set; }

    public NavigationAgent2D NavAgent;

    public bool IsMoving { get; set; }

    public override void _Ready()
    {
        NavAgent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        NinePatchRect = GetNode<NinePatchRect>("NinePatchRect");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (NavAgent.IsNavigationFinished()) return;

        var currentAgentPosition = GlobalTransform.Origin;
        var nextPathPosition = NavAgent.GetNextPathPosition();

        Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * _movementSpeed;

        MoveAndSlide();
    }

    public void SetTargetPosition(Vector2 position)
    {
        NavAgent.SetTargetPosition(position);
    }
}