using Godot;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Scripts.Managers;

public partial class Terminal : Node2D
{
    [Export] private string TextAction = "[E]";
    [Export] private int LevelNumber;
    [Export] private PackedScene TimerScene;
    private Area2D SaveArea => GetNode<Area2D>("SaveArea");
    private MarginContainer MarginContainer => GetNode<MarginContainer>("MarginContainer");
    private Label LabelTextAction => MarginContainer.GetNode<Panel>("Panel").GetNode<Label>("Label");
    private AnimatedSprite2D AnimatedSprite2D => GetNode<AnimatedSprite2D>("Sprite2D");
    private MyhaPlayer MyhaPlayer;
    private SaveSystem SaveSystem;
    private PlayerManager PlayerManager;
    private GameManager GameManager;
    private bool _hasBeenUsed = false;

    public override void _Ready()
    {
        SignalManager.Instance.PlayerSaveTheGame += OnPlayerSaveTheGame;
        SaveSystem = SaveSystem.SaveSystemInstance;
        PlayerManager = PlayerManager.GetPlayerGlobalInstance();
        this.AnimatedSprite2D.Play("idle");
        GameManager = GameManager.GetGameManagerInstance();
    }

    private void OnPlayerSaveTheGame()
    {
        this._hasBeenUsed = true;
        this.AnimatedSprite2D.Play("saving");
    }

    private void _on_sprite_2d_animation_finished()
    {
        this.AnimatedSprite2D.Play("saved");
        if (MyhaPlayer != null
            && MyhaPlayer.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_SAVE
            && PlayerManager.PlayerCanSaveTheGame == true)
        {
            MyhaPlayer.SetCurrentEnabledAction(PrototipoMyha.Player.StateManager.PlayerCurrentEnabledAction.NONE);
            PlayerManager.PlayerCanSaveTheGame = false;
        }
    }



    public void _on_save_area_body_entered(Node2D body)
    {
        this.MarginContainer.Visible = true;
        this.LabelTextAction.Text = TextAction;
        if (body.IsInGroup("player"))
        {
            MyhaPlayer = body as MyhaPlayer;
            MyhaPlayer.ProcessMode = ProcessModeEnum.Pausable;
            if (!_hasBeenUsed)
            {
                MyhaPlayer.SetCurrentEnabledAction(PlayerCurrentEnabledAction.CAN_SAVE);
                PlayerManager.PlayerCanSaveTheGame = true;
                PlayerManager.UpdatePlayerPosition(this.GlobalPosition);
            }
        }

    }

    public void _on_save_area_body_exited(Node2D body)
    {
        this.MarginContainer.Visible = false;
        if (body.IsInGroup("player"))
        {
            PlayerManager.GetPlayerGlobalInstance().PlayerCanSaveTheGame = false;
            MyhaPlayer.SetCurrentEnabledAction(PlayerCurrentEnabledAction.NONE);
        }

    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("action")
             && MyhaPlayer != null
             && MyhaPlayer.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_SAVE)
        {
            if (MyhaPlayer != null)
                SaveSystem.SaveGame(LevelNumber);

            this.LabelTextAction.Text = "Jogo Salvo!";
        }
    }
}
