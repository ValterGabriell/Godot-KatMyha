using Godot;
using KatMyha.Scripts.Managers;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Utilidades;

public partial class CameraTurnOnOff : Node2D
{

    [Export] private Area2D Area2D;
    [Export] private float SizeOfDetection;
    [Export] private Timer TimerToTurnOnAndOff;
    [Export] private float TimeToTurnOnAndOff = 2.0f;

    private SoundManager SoundManager;
    private SignalManager SignalManager;
    private bool hasEmittedKillSignal = false;

    private bool isTurnOff = false;

    public override void _Ready()
    {
        SoundManager = SoundManager.Instance;
        SignalManager = SignalManager.Instance;
        Area2D.BodyEntered += OnBodyEntered;
        UpdateDetectionRayLength();
        TimerToTurnOnAndOff.WaitTime = TimeToTurnOnAndOff;
        TimerToTurnOnAndOff.Timeout += OnTimerTimeout;
    }

    private void UpdateDetectionRayLength()
    {
        var childrens = Area2D.GetChildren();
        foreach (var child in childrens)
        {
            if (child is CollisionShape2D collisionShape)
            {
                if (collisionShape.Shape is SeparationRayShape2D separationRay)
                {
                    separationRay.Length = SizeOfDetection;
                }
            }
        }
    }

    private void UpdateDetectionState()
    {
        var childrens = Area2D.GetChildren();
        foreach (var child in childrens)
        {
            if (child is CollisionShape2D collisionShape)
            {
                collisionShape.Disabled = isTurnOff;
            }
        }
    }

    private void OnTimerTimeout()
    {
        GDLogger.LogBlue("Timer timeout reached. Toggling camera state.");
        isTurnOff = !isTurnOff;
        UpdateDetectionState();
        TimerToTurnOnAndOff.Start();
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("player") && !hasEmittedKillSignal)
        {
            MyhaPlayer player = body as MyhaPlayer;
            if (player != null)
            {
                ProcessKill(player);
            }
        }
    }

    private void ProcessKill(MyhaPlayer player)
    {
        SignalManager.EmitSignal(nameof(SignalManager.EnemyKillMyha));
        SoundManager.PlaySound(player.DeathAudioStreamPlayer2D);
        hasEmittedKillSignal = true;
    }
}
