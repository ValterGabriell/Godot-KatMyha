using Godot;
using KatrinaGame.Players;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Utilidades;
using System;

public partial class OpenFloorDoor : StaticBody2D
{
    private Area2D OpenArea;
    private MyhaPlayer myhaPlayer;

    public override void _Ready()
    {
        OpenArea = GetNode<Area2D>("OpenArea");
        if(OpenArea != null)
        {
            OpenArea.BodyEntered += OnBodyEntered;
            OpenArea.BodyExited += OnBodyExited;
        }
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is MyhaPlayer)
        {
            myhaPlayer = body as MyhaPlayer;
            myhaPlayer.SetCurrentEnabledAction(PlayerCurrentEnabledAction.NONE);
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        if(body is MyhaPlayer)
        {
            myhaPlayer = body as MyhaPlayer;
            myhaPlayer.SetCurrentEnabledAction(PlayerCurrentEnabledAction.CAN_OPEN_FLOOR_DOOR);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("action")
             && myhaPlayer != null
             && myhaPlayer.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_OPEN_FLOOR_DOOR)
        {
            myhaPlayer.GlobalPosition += new Vector2(0, 32);
        }
    }
}
