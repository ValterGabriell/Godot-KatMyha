using Godot;
using Godot.Collections;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Utilidades;
using System.Linq;


public partial class DoorSubFase : Node2D
{
    [Export] private Area2D AreaToChangeScene { get; set; }
    [Export] private Texture2D DoorSprite { get; set; }
    [Export] private Panel Panel { get; set; }
    [Export] private PlayerSubphaseKey CurrentSubFaseIndex { get; set; }
    [Export] private PackedScene ElevatorScene { get; set; }
    [Export] private Array<Marker2D> PathToFollow { get; set; } = [];
    [Export] private float ElevatorMoveSpeed { get; set; } = 150f;

    private MyhaPlayer Player;
    private PlayerManager PlayerManager;
    private SignalManager SignalManager;


    public override void _Ready()
    {
        this.AreaToChangeScene.BodyEntered += OnAreaToChangeSceneBodyEntered;
        this.AreaToChangeScene.BodyExited += OnAreaToChangeSceneBodyExited;
        this.GetNode<Sprite2D>("Sprite2D").Texture = DoorSprite;
        this.Panel.Visible = false;
        this.PlayerManager = PlayerManager.GetPlayerGlobalInstance();
        this.SignalManager = SignalManager.Instance;
    }


    private void OnAreaToChangeSceneBodyExited(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            this.Panel.Visible = false;
            Player = body as MyhaPlayer;
            Player.SetCurrentEnabledAction(PlayerCurrentEnabledAction.NONE);
            Player = null;
        }
    }

    private void OnAreaToChangeSceneBodyEntered(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            this.Panel.Visible = true;
            Player = body as MyhaPlayer;
            Player.SetCurrentEnabledAction(PlayerCurrentEnabledAction.CAN_CHANGE_SCENE);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("action")
            && this.Player != null
            && this.Player.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_CHANGE_SCENE)
        {
            if (CurrentSubFaseIndex == PlayerSubphaseKey.SUBFASE_1
                && PlayerManager.IsPlayerSubphaseKeyUnlocked(PlayerSubphaseKey.SUBFASE_1))
            {
                SpawnElevatorAndChangeSubphase();
                return;
            }

            if (CurrentSubFaseIndex == PlayerSubphaseKey.BACK_TO_BEGIN
               && PlayerManager.IsPlayerSubphaseKeyUnlocked(PlayerSubphaseKey.BACK_TO_BEGIN))
            {
                SpawnElevatorAndChangeSubphase();
                return;
            }

            if (CurrentSubFaseIndex == PlayerSubphaseKey.SUBFASE_2
                && PlayerManager.IsPlayerSubphaseKeyUnlocked(PlayerSubphaseKey.SUBFASE_2))
            {
                GDLogger.Log("Player has key for subfase 2");

                return;
            }

            if (CurrentSubFaseIndex == PlayerSubphaseKey.SUBFASE_3
                && PlayerManager.IsPlayerSubphaseKeyUnlocked(PlayerSubphaseKey.SUBFASE_3))
            {
                GDLogger.Log("Player has key for subfase 3");

                return;
            }

            GDLogger.LogError("Player has no key");

        }


    }

    private void SpawnElevatorAndChangeSubphase()
    {
        var elevatorInstance = ElevatorScene.Instantiate<Elevator>();
        elevatorInstance.MoveSpeed = ElevatorMoveSpeed;
        elevatorInstance.GlobalPosition = this.GlobalPosition;
        this.GetParent().AddChild(elevatorInstance);
        SignalManager.PlayerAcessSubphase.Invoke(PathToFollow.ToList());
    }
}
