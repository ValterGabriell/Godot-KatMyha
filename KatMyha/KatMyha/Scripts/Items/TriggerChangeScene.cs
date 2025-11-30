using Godot;
using KatrinaGame.Players;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Utilidades;
using System;

public partial class TriggerChangeScene : Area2D
{
    
    [ExportGroup("General Settings")]   
    [Export] private float DelayToStart { get; set; } = 0f;
    [Export] private MyhaPlayer MyhaPlayerInit;

    [ExportCategory("Player Settings")]
    [Export] private bool FreezePlayer { get; set; } = true;
    [Export] private float ProcessDelayInSeconds { get; set; } = 0f;

    [ExportGroup("Camera Interpolation Settings")]
    [Export] private float NewZoomX { get; set; } = 1.5f;
    [Export] private float NewZoomY { get; set; } = 1.5f;
    [Export] private float NewPositionY { get; set; } = 0f;
    [Export] private float NewPositionX { get; set; } = 0f;

    [ExportGroup("New Camera")]
    [Export] private bool FreezeCameraAfterInterpolation { get; set; } = false;
    [Export] private Camera2D NewCamera { get; set; }

    private Vector2 CameraZoomBeforeChange = Vector2.One;
    private Vector2 CameraPosBeforeChange = Vector2.One;

    public override void _Ready()
    {
        var camera = MyhaPlayerInit.GetNode<Camera2D>("Camera2D");
        CameraZoomBeforeChange = camera.Zoom;
         CameraPosBeforeChange = camera.Position;
    }

    public void _on_body_entered(Node2D node2D)
    {
        if (node2D.IsInGroup("player"))
        {
            MyhaPlayer myhaPlayer = node2D as MyhaPlayer;
            if (FreezePlayer && myhaPlayer != null)
            {
                myhaPlayer.SetState(PlayerState.IDLE);
                myhaPlayer.BlockMovement();
            }
            GetTree().CreateTimer(DelayToStart).Timeout += () => StartCameraInterpolation(myhaPlayer);
         
        }
    }

    public void _on_body_exited(Node2D node2D)
    {
        if (node2D.IsInGroup("player"))
        {
            MyhaPlayer myhaPlayer = node2D as MyhaPlayer;
            GetTree().CreateTimer(DelayToStart).Timeout += () => EndCameraInterpolation(myhaPlayer);
        }
    }

    private void StartCameraInterpolation(MyhaPlayer myhaPlayer)
    {
        GDLogger.LogGreen("Starting Camera Interpolation");
        var camera = myhaPlayer.GetNode<Camera2D>("Camera2D");
        var tween = GetTree().CreateTween();
        var targetZoom = camera.Zoom + new Vector2(NewZoomX, NewZoomY);
        var targetPosition = camera.Position + new Vector2(NewPositionX, NewPositionY);
        float duration = ProcessDelayInSeconds;
        tween.TweenProperty(camera, "position", targetPosition, duration)
         .SetTrans(Tween.TransitionType.Sine)
         .SetEase(Tween.EaseType.InOut);

        tween.TweenProperty(camera, "zoom", targetZoom, duration)
           .SetTrans(Tween.TransitionType.Sine)
           .SetEase(Tween.EaseType.InOut);

  
        tween.Finished += () => OnInterpolationFinished(myhaPlayer, camera);
    }

    private void EndCameraInterpolation(MyhaPlayer myhaPlayer)
    {
        FreezeCameraAfterInterpolation = false;
      
        var camera = myhaPlayer.GetNode<Camera2D>("Camera2D");
        var tween = GetTree().CreateTween();

        var targetZoom = CameraZoomBeforeChange;
        var targetPosition = CameraPosBeforeChange;

        float duration = ProcessDelayInSeconds;
        tween.TweenProperty(camera, "position", targetPosition, duration)
         .SetTrans(Tween.TransitionType.Sine)
         .SetEase(Tween.EaseType.InOut);

        tween.TweenProperty(camera, "zoom", targetZoom, duration)
           .SetTrans(Tween.TransitionType.Sine)
           .SetEase(Tween.EaseType.InOut);

        if (!FreezeCameraAfterInterpolation)
        {
            camera.MakeCurrent();
        }

        QueueFree();
    }

    public virtual void OnInterpolationFinished(MyhaPlayer myhaPlayer, Camera2D playerCamera)
    {
        if (FreezeCameraAfterInterpolation && NewCamera != null)
        {
            NewCamera.GlobalPosition = playerCamera.GlobalPosition;
            NewCamera.Zoom = playerCamera.Zoom;
            NewCamera.MakeCurrent();
        }

        if (FreezePlayer)
        {
            myhaPlayer.UnblockMovement();
        }
    }
}
