using Godot;
using KatMyha.Scripts.Items.KillLight;

public partial class HorizontalMovementRail : FallTrap
{
    [Export] private float MoveSpeed { get; set; } = 100f;
    private Vector2 markerA;
    private Vector2 markerB;
    private Vector2 currentTarget;
    private bool isInitialized = false;
    private bool isTargetingMarkerA = false;
    private bool isLoadingFromSave = false;

    public override void _Ready()
    {
        if (!isLoadingFromSave)
        {
            InitializeMarkers();
        }
    }

    private void InitializeMarkers()
    {
        if (RoamMarkerA == null || RoamMarkerB == null)
        {
            GD.PrintErr($"[HorizontalMovementRail] InitializeMarkers FALHOU: RoamMarkerA={RoamMarkerA}, RoamMarkerB={RoamMarkerB}");
            isInitialized = false;
            return;
        }
        this.markerA = RoamMarkerA.GlobalPosition;
        this.markerB = RoamMarkerB.GlobalPosition;
        float distToA = this.GlobalPosition.DistanceTo(markerA);
        float distToB = this.GlobalPosition.DistanceTo(markerB);
        currentTarget = distToA < distToB ? markerB : markerA;
        isTargetingMarkerA = (currentTarget == markerA);
        isInitialized = true;
        GD.Print($"[HorizontalMovementRail] Inicializado! MarkerA={markerA}, MarkerB={markerB}, CurrentTarget={currentTarget}, IsTargetingMarkerA={isTargetingMarkerA}");
    }

    public override void ReinitializeMarkers()
    {
        GD.Print($"[HorizontalMovementRail] ReinitializeMarkers chamado. RoamMarkerA={RoamMarkerA}, RoamMarkerB={RoamMarkerB}");
        isLoadingFromSave = false;
        InitializeMarkers();
    }

    public override bool GetIsTargetingMarkerA()
    {
        return isTargetingMarkerA;
    }

    public override void SetTargetMarker(bool isTargetingA)
    {
        if (RoamMarkerA == null || RoamMarkerB == null)
        {
            GD.PrintErr("[HorizontalMovementRail] SetTargetMarker: Marcadores não disponíveis");
            return;
        }
        
        this.markerA = RoamMarkerA.GlobalPosition;
        this.markerB = RoamMarkerB.GlobalPosition;
        currentTarget = isTargetingA ? markerA : markerB;
        isTargetingMarkerA = isTargetingA;
        isInitialized = true;
        
        GD.Print($"[HorizontalMovementRail] SetTargetMarker: Target definido para {(isTargetingA ? "MarkerA" : "MarkerB")} = {currentTarget}");
    }
    
    public void PrepareForLoad()
    {
        isLoadingFromSave = true;
        isInitialized = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!isInitialized || isLoadingFromSave)
        {
            return;
        }

        var direction = (currentTarget - this.GlobalPosition).Normalized();
        var newPos = direction * MoveSpeed * (float)delta;

        this.GlobalPosition += newPos;

        if (this.GlobalPosition.DistanceTo(currentTarget) < 15.0f)
        {
            currentTarget = currentTarget == markerA ? markerB : markerA;
            isTargetingMarkerA = (currentTarget == markerA);
        }
    }
}
