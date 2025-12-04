using Godot;
using KatrinaGame.Players;
using System;

public partial class ChangeCamera : Camera2D
{
    [Export] private Area2D TriggerMarker;
    [Export] private MyhaPlayer MyhaPlayer;

    private bool isInsideTrigger = false;

    public override void _Ready()
    {
        this.TriggerMarker.BodyEntered += OnGetTriggerMarker;
        this.TriggerMarker.BodyExited += OnExitTriggerMarker;
    }

    private void OnGetTriggerMarker(Node2D body)
    {
        if (!isInsideTrigger)
        {
            isInsideTrigger = true;
            ChangeCameraOnGetTriggerMarker();
        }
    }

    private void OnExitTriggerMarker(Node2D body)
    {
        if (isInsideTrigger)
        {
            isInsideTrigger = false;
            ResetCamera();
        }
    }

    private void ChangeCameraOnGetTriggerMarker()
    {
        var directionThatPlayerIsLookingAt = MyhaPlayer.GetDirectionThatPlayerIsLookingAt();
        var cameraWidth = GetViewportRect().Size.X / Zoom.X;

        if (directionThatPlayerIsLookingAt == PlayerDirection.Right)
        {
            this.Offset = new Vector2(cameraWidth, 0);
        }
        else
        {
            this.Offset = new Vector2(-cameraWidth, 0);
        }
    }

    private void ResetCamera()
    {
        this.Offset = Vector2.Zero;
    }
}
