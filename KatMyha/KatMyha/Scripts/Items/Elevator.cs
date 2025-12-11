using Godot;
using Godot.Collections;
using PrototipoMyha;
using PrototipoMyha.Utilidades;
using System.Collections.Generic;

public partial class Elevator : Node2D
{
    private SignalManager SignalManager;
    private Array<Marker2D> PathToFollow { get; set; } = [];
    private bool StartedSceneChange = false;
    private int CurrentMarkerIndex = 0;

    public float MoveSpeed { get; set; } = 100f;

    public override void _Ready()
    {
        SignalManager = SignalManager.Instance;
        SignalManager.PlayerAcessSubphase += OnPlayerAcessSubphase;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!StartedSceneChange || PathToFollow.Count == 0)
        {
            return;
        }

        if (CurrentMarkerIndex >= PathToFollow.Count)
        {
            GDLogger.Log("Elevator reached final destination!");

            SignalManager.ElevatorReachFinalPoint.Invoke(this.GlobalPosition);
            StartedSceneChange = false;
            this.QueueFree();
            return;
        }

        var currentMarker = PathToFollow[CurrentMarkerIndex];
        var distance = this.GlobalPosition.DistanceTo(currentMarker.GlobalPosition);

        if (distance >= 5.0f)
        {
            var direction = (currentMarker.GlobalPosition - this.GlobalPosition).Normalized();
            var moveAmount = MoveSpeed * (float)delta;

            if (moveAmount > distance)
            {
                this.GlobalPosition = currentMarker.GlobalPosition;
            }
            else
            {
                this.GlobalPosition += direction * moveAmount;
            }
        }
        else
        {
            // Chegou no marker atual, vai para o próximo
            GDLogger.Log($"Reached marker {CurrentMarkerIndex + 1}!");
            CurrentMarkerIndex++;
        }
    }

    private void OnPlayerAcessSubphase(List<Marker2D> list)
    {
        GetNode<Camera2D>("Camera2D").MakeCurrent();
        GDLogger.Log($"Player accessing subphase via elevator. Markers: {list.Count}");

        if (list.Count == 0)
        {
            GDLogger.LogError("PathToFollow is empty!");
            return;
        }

        StartedSceneChange = true;
        CurrentMarkerIndex = 0;
        PathToFollow = new Array<Marker2D>(list);

        // Log das posições dos markers para debug
        for (int i = 0; i < PathToFollow.Count; i++)
        {
            GDLogger.Log($"Marker {i + 1} position: {PathToFollow[i].GlobalPosition}");
        }

        GDLogger.Log($"Elevator starting position: {this.GlobalPosition}");
    }
}
