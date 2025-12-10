using Godot;

public partial class LaserHorizontalRail : HorizontalMovementRail
{
    [Export] private float SizeOfDetection { get; set; } = 150f;
    [Export] private CollisionShape2D SeparationRayShape2D { get; set; } = null;

    public override void _Ready()
    {
        base._Ready();
        if (SeparationRayShape2D.Shape is SeparationRayShape2D separationRay)
            separationRay.Length = SizeOfDetection;

    }
}
