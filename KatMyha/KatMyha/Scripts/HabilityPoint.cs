using Godot;
using KatrinaGame.Players;
using PrototipoMyha.Player.StateManager;

public partial class HabilityPoint : Area2D
{
    private PlayerManager playerManager;
    private MyhaPlayer myhaPlayer;
    [Export] private PlayerHabilityKey HabilityKey;
    [Export] private Label Label;
    public override void _Ready()
    {
        this.BodyEntered += OnBodyEntered;
        this.BodyExited += OnBodyExited;
    }

    private void OnBodyExited(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            Label.Visible = false;
            myhaPlayer.SetCurrentEnabledAction(PrototipoMyha.Player.StateManager.PlayerCurrentEnabledAction.NONE);
            myhaPlayer = null;
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            Label.Visible = true;
            myhaPlayer = body as MyhaPlayer;
            myhaPlayer.SetCurrentEnabledAction(PrototipoMyha.Player.StateManager.PlayerCurrentEnabledAction.CAN_GET_HABILITY_POINT);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("action")
            && myhaPlayer != null
            && myhaPlayer.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_GET_HABILITY_POINT)
        {
            playerManager = PlayerManager.GetPlayerGlobalInstance();
            if (playerManager != null)
            {
                playerManager.UnlockPlayerHability(HabilityKey);
                this.QueueFree();
            }
        }
    }
}
