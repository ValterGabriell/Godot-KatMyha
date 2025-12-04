using Godot;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Scripts.Managers;
using System;

public partial class Terminal : Node2D
{
    private Area2D SaveArea => GetNode<Area2D>("SaveArea");
    private AnimatedSprite2D AnimatedSprite2D => GetNode<AnimatedSprite2D>("Sprite2D");
    private MyhaPlayer MyhaPlayer;
    private GameManager GameManager;
    private PlayerManager PlayerManager;

    private bool _hasBeenUsed = false;

    public override void _Ready()
    {
       SignalManager.Instance.PlayerSaveTheGame += OnPlayerSaveTheGame;
       GameManager = GameManager.GetGameManagerInstance();
        PlayerManager = PlayerManager.GetPlayerGlobalInstance();
        this.AnimatedSprite2D.Play("idle");
    }

    private void OnPlayerSaveTheGame()
    {
        this._hasBeenUsed = true;
        this.AnimatedSprite2D.Play("saving");
    }

    private void _on_sprite_2d_animation_finished()
    {
        this.AnimatedSprite2D.Play("saved");
        if(MyhaPlayer != null 
            && MyhaPlayer.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_SAVE
            && PlayerManager.PlayerCanSaveTheGame == true)
        {
            MyhaPlayer.SetCurrentEnabledAction(PrototipoMyha.Player.StateManager.PlayerCurrentEnabledAction.NONE);
            PlayerManager.PlayerCanSaveTheGame = false;
        }
    }



    public void _on_save_area_body_entered(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            MyhaPlayer = body as MyhaPlayer;
            if (!_hasBeenUsed)
            {
                MyhaPlayer.SetCurrentEnabledAction(PrototipoMyha.Player.StateManager.PlayerCurrentEnabledAction.CAN_SAVE);
                PlayerManager.PlayerCanSaveTheGame = true;
                PlayerManager.UpdatePlayerPosition(this.GlobalPosition);
            }
        }
   
    }

    public void _on_save_area_body_exited(Node2D body)
    {
        PlayerManager.GetPlayerGlobalInstance().PlayerCanSaveTheGame = false;
        MyhaPlayer.SetCurrentEnabledAction(PrototipoMyha.Player.StateManager.PlayerCurrentEnabledAction.NONE);
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("action")
             && MyhaPlayer != null
             && MyhaPlayer.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_SAVE)
        {
            if (MyhaPlayer != null)
                GameManager.SaveGame();

        }
    }
}
