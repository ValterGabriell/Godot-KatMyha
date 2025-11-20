using Godot;
using KatMyha.Scripts.Items.KillLight;
using KatMyha.Scripts.Utils;
using PrototipoMyha;
using PrototipoMyha.Enemy;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;



public partial class Bullet01 : RigidBody2D
{
    [Export] public float LifeTime = 0.5f;
    [Export] public AudioStreamPlayer2D ImpactSound;
    private List<EnemyBase> allEnemiesOnRange = [];

    private bool hasLanded = false;

    public override void _Ready()
    {
        var area2D = GetNode<Area2D>("Area2D");
        // Detectar colisão com chão
        area2D.BodyEntered += OnBodyEntered;
    }


    private void OnBodyEntered(Node body)
    {
        if (!hasLanded)
        {
            hasLanded = true;
            
            // Congelar a física usando CallDeferred para evitar erro durante flushing queries
            //CallDeferred(PropertyName.Freeze, true);
            
            CallDeferred(nameof(ProcessLanding));
        }
    }

    private void ProcessLanding()
    {
        var landingPosition = this.GlobalPosition;


        PlayerManager.GetPlayerGlobalInstance().UpdateLastDistractionBallPosition(landingPosition);

        var allEnemies = GetTree().GetNodesInGroup(EnumGroups.enemy.ToString());
        ImpactSound?.Play();

        List<EnemyBase> enemiesInRange = FindItemsNearest.FindAndSetAimTargets(allEnemies, landingPosition, 150f, allEnemiesOnRange);
        foreach (var enemy in enemiesInRange)
        {
            enemy.SetState(PrototipoMyha.Enemy.States.EnemyState.DistractionAlerted);
        }

        // Destruir após X segundos
        var timer = GetTree().CreateTimer(LifeTime);
        timer.Timeout += QueueFree;
    }
}
