using Godot;
using KatMyha.Scripts.Managers;
using KatrinaGame.Core;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;

public partial class FreezeCamera : Node2D
{
    [Export] private Area2D Area2D;
    [Export] private float SizeOfDetection;
    private SoundManager SoundManager;
    private SignalManager SignalManager;
    private bool hasEmittedKillSignal = false;

    public override void _Ready()
    {
        SoundManager = SoundManager.Instance;
        SignalManager = SignalManager.Instance;
        Area2D.BodyEntered += OnBodyEntered;
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

    private void OnBodyEntered(Node2D body)
    {
      if (body.IsInGroup("player") && !hasEmittedKillSignal)
        {
            MyhaPlayer player = body as MyhaPlayer;
            if (player != null && player.CurrentLightHiddenState == PrototipoMyha.Player.StateManager.MyhaContactLightHiddenState.MYHA_IS_ON_LIGHT)
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
