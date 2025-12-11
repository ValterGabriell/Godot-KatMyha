using Godot;
using KatMyha.Scripts.Managers;
using KatrinaGame.Players;
using PrototipoMyha;

public partial class LaserHorizontalRail : HorizontalMovementRail
{
    [Export] private float SizeOfDetection { get; set; } = 150f;
    [Export] private CollisionShape2D SeparationRayShape2D { get; set; } = null;
    [Export] private Area2D DetectionArea2D { get; set; } = null;
    private SoundManager SoundManager;
    private SignalManager SignalManager;
    private bool hasEmittedKillSignal = false;

    public override void _Ready()
    {
        base._Ready();
        SoundManager = SoundManager.Instance;
        SignalManager = SignalManager.Instance;
        if (SeparationRayShape2D.Shape is SeparationRayShape2D separationRay)
            separationRay.Length = SizeOfDetection;

        DetectionArea2D.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("player") && !hasEmittedKillSignal)
        {
            MyhaPlayer player = body as MyhaPlayer;
            if (player.CurrentPlayerState != PrototipoMyha.Player.StateManager.PlayerState.HIDDEN)
                ProcessKill(player);
        }
    }

    private void ProcessKill(MyhaPlayer player)
    {
        SignalManager.EmitSignal(nameof(SignalManager.EnemyKillMyha));
        SoundManager.PlaySound(player.DeathAudioStreamPlayer2D);
        hasEmittedKillSignal = true;
        GetTree().CreateTimer(1.0f).Timeout += () =>
        {
            hasEmittedKillSignal = false;
        };
    }
}
