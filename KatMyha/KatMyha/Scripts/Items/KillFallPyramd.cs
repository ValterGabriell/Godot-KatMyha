using Godot;
using KatMyha.Scripts.Managers;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Utilidades;
using System;

public partial class KillFallPyramd : Node2D
{
    private SignalManager SignalManager;
    private SoundManager SoundManager = SoundManager.Instance;
    private bool hasEmittedKillSignal = false;

    [Export] private float FallDelayInSeconds { get; set; } = 0.5f;
    [Export] private float FallPosY { get; set; } = 4000;
    [Export] private float LenghtOfRay { get; set; } = 150;
    [Export] private CollisionShape2D Ray { get; set; }
    public Guid InstanceID { get; set; } = Guid.NewGuid();

    [Signal]
    private delegate void EnemyPyramdHasToFallEventHandler();

    public override void _Ready()
    {
        SignalManager = SignalManager.Instance;
        EnemyPyramdHasToFall += OnEnemyPyramdHasToFall;
        if (Ray.Shape is SeparationRayShape2D)
        {
            var separationRay = Ray.Shape as SeparationRayShape2D;
            separationRay.Length = LenghtOfRay;
        }


    }

    private void OnEnemyPyramdHasToFall()
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(this, "position:y", FallPosY, FallDelayInSeconds);
        tween.Play();

        GetTree().CreateTimer(FallDelayInSeconds).Timeout += () =>
        {
            QueueFree();
        };
    }

    public void _on_area_2d_body_entered(Node2D node2D)
    {
        if (node2D.IsInGroup("player"))
        {
            ProcessKillOfPlayer(node2D as MyhaPlayer);
        }
    }

    public void _on_kill_player_area_body_entered(Node2D node2D)
    {
        if (node2D.IsInGroup("player"))
        {
            GDLogger.LogError("Player has entered kill area of pyramid.");
            EmitSignal(nameof(EnemyPyramdHasToFall));
        }
    }




    private void ProcessKillOfPlayer(MyhaPlayer player)
    {
        SignalManager.EmitSignal(nameof(SignalManager.EnemyKillMyha));
        SoundManager.PlaySound(player.DeathAudioStreamPlayer2D);
        hasEmittedKillSignal = true;
    }
}
