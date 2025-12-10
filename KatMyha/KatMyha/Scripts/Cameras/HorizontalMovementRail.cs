using Godot;

public partial class HorizontalMovementRail : Node2D
{
    /// <summary>
    /// Gets or sets the primary roaming marker used for navigation or positioning.
    ///  <summary>
    [Export] private Marker2D RoamMarkerA { get; set; } = null;

    /// <summary>
    /// Gets or sets the secondary roaming marker used for navigation or positioning.
    /// </summary>
    [Export] private Marker2D RoamMarkerB { get; set; } = null;
    [Export] private float MoveSpeed { get; set; } = 100f;
    private Vector2 markerA;
    private Vector2 markerB;
    private Vector2 currentTarget;

    public override void _Ready()
    {
        this.markerA = this.RoamMarkerA.GlobalPosition;
        this.markerB = this.RoamMarkerB.GlobalPosition;
        float distToA = this.GlobalPosition.DistanceTo(markerA);
        float distToB = this.GlobalPosition.DistanceTo(markerB);
        currentTarget = distToA < distToB ? markerB : markerA;
    }

    public override void _PhysicsProcess(double delta)
    {
        var direction = (currentTarget - this.GlobalPosition).Normalized();
        var newPos = direction * MoveSpeed * (float)delta;

        this.GlobalPosition += newPos;


        if (this.GlobalPosition.DistanceTo(currentTarget) < 15.0f)
        {
            currentTarget = currentTarget == markerA ? markerB : markerA;
        }

    }

}
