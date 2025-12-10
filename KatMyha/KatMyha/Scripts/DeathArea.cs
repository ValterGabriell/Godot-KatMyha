using Godot;
using KatMyha.Scripts.Managers;
using KatrinaGame.Players;
using PrototipoMyha;

public partial class DeathArea : Area2D
{
    private SignalManager SignalManager;
    private SoundManager SoundManager;
    private bool hasEmittedKillSignal = false;
    public override void _Ready()
    {
        SignalManager = SignalManager.Instance;
        SoundManager = SoundManager.Instance;
        this.BodyEntered += OnBodyEntered;

    }

    private void OnBodyEntered(Node2D body)
    {
        if (body != null && body.IsInGroup("player"))
        {
            var player = body as MyhaPlayer;
            ProcessKill(player);
        }
    }

    private void ProcessKill(MyhaPlayer player)
    {
        SignalManager.EmitSignal(nameof(SignalManager.EnemyKillMyha));
        SoundManager.PlaySound(player.DeathAudioStreamPlayer2D);
        hasEmittedKillSignal = true;
    }
}
