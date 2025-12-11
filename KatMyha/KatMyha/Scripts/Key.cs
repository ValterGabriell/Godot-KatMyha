using Godot;
using KatrinaGame.Players;
using PrototipoMyha.Player.StateManager;

public partial class Key : Node2D
{
    private PlayerManager playerManager;
    private MyhaPlayer myhaPlayer;
    [Export] private Label Label;
    [Export] private Area2D Area2d;
    [Export] private PlayerSubphaseKey CurrentSubFaseIndex { get; set; }
    public override void _Ready()
    {
        this.Area2d.BodyEntered += OnBodyEntered;
        this.Area2d.BodyExited += OnBodyExited;
        playerManager = PlayerManager.GetPlayerGlobalInstance();
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
            myhaPlayer.SetCurrentEnabledAction(PrototipoMyha.Player.StateManager.PlayerCurrentEnabledAction.CAN_GET_KEY);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("action")
            && myhaPlayer != null
            && playerManager != null
            && myhaPlayer.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_GET_KEY)
        {
            if (playerManager != null)
            {
                playerManager.UnlockPlayerSubphaseKey(CurrentSubFaseIndex);
                this.QueueFree();
            }
        }
    }
}
