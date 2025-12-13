using Godot;
using KatMyha.Scripts.Managers;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Player.StateManager;

public partial class KillYourself : Area2D
{
    [Export] private Label Label;
    private MyhaPlayer MyhaPlayer;

    private SignalManager SignalManager;
    private SoundManager SoundManager;
    private bool hasEmittedKillSignal = false;
    public override void _Ready()
    {
        SignalManager = SignalManager.Instance;
        SoundManager = SoundManager.Instance;
        this.BodyEntered += OnBodyEntered;
        this.BodyExited += OnBodyExited;
    }

    private void OnBodyExited(Node2D body)
    {
        if (body != null && body.IsInGroup("player"))
        {
            MyhaPlayer = body as MyhaPlayer;
            Label.Visible = false;
            MyhaPlayer.SetCurrentEnabledAction(PlayerCurrentEnabledAction.NONE);
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body != null && body.IsInGroup("player"))
        {
            MyhaPlayer = body as MyhaPlayer;
            MyhaPlayer.SetCurrentEnabledAction(PlayerCurrentEnabledAction.CAN_KILL_YOURSELF);
            Label.Visible = true;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("action")
           && MyhaPlayer != null
           && MyhaPlayer.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_KILL_YOURSELF)
        {
            ProcessKill();
        }
    }

    private void ProcessKill()
    {
        SignalManager.EmitSignal(nameof(SignalManager.EnemyKillMyha));
        SoundManager.PlaySound(MyhaPlayer.DeathAudioStreamPlayer2D);
        hasEmittedKillSignal = true;
    }
}
