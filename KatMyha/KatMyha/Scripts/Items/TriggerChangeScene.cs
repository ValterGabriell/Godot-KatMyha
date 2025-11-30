using Godot;
using KatrinaGame.Players;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Utilidades;
using System;

public partial class TriggerChangeScene : Area2D
{
    [Export] private MyhaPlayer MyhaPlayer { get; set; }
    [Export] private float DelayToStart { get; set; } = 0.2f;
    [Export] private bool FreezePlayer { get; set; } = true;
    [Export] private float ProcessDelayInSeconds { get; set; } = 5.5f;
    [Export] private float TargetInterpolationZoomX { get; set; } = 0.5f;
    [Export] private float TargetInterpolationZoomY { get; set; } = 0.5f;
    [Export] private float TargetInterpolationPositionY { get; set; } = 0.5f;
    [Export] private float TargetInterpolationPositionX { get; set; } = 0.5f;

    

    public void _on_body_entered(Node2D node2D)
    {
        if (node2D.IsInGroup("player"))
        {
            if (FreezePlayer)
            {
                MyhaPlayer.SetState(PlayerState.IDLE);
                MyhaPlayer.BlockMovement();
            }
            GetTree().CreateTimer(DelayToStart).Timeout += () => StartCameraInterpolation();
         
        }
    }

    private void StartCameraInterpolation()
    {
        var camera = MyhaPlayer.GetNode<Camera2D>("Camera2D");
        var tween = GetTree().CreateTween();
        // Zoom final desejado (exemplo: aumenta 0.5 em cada eixo)
        var targetZoom = camera.Zoom + new Vector2(TargetInterpolationZoomX, TargetInterpolationZoomY);
        var targetPosition = camera.Position + new Vector2(TargetInterpolationPositionX, TargetInterpolationPositionY);
        // Duração do efeito (em segundos)
        float duration = ProcessDelayInSeconds;
        tween.TweenProperty(camera, "position", targetPosition, duration)
         .SetTrans(Tween.TransitionType.Sine)
         .SetEase(Tween.EaseType.InOut);

        tween.TweenProperty(camera, "zoom", targetZoom, duration)
           .SetTrans(Tween.TransitionType.Sine)
           .SetEase(Tween.EaseType.InOut);

        if (FreezePlayer)
        {
            MyhaPlayer.UnblockMovement();
        }

        QueueFree();

    }
}
