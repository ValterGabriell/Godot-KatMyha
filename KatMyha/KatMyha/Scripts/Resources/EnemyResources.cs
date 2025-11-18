using Godot;
using PrototipoMyha.Enemy.Components.Impl.EnemyMovement.Strategies.PatrolHandler;
using PrototipoMyha.Enemy.Components.Interfaces;
using System;
using System.Collections.Generic;

public partial class EnemyResources : Resource
{
    [ExportGroup("Base")]
    [Export] public float Health = 100;

    [Export] public int DamageAmount = 10;

    [Export] public float MoveSpeed = 900f;

    [Export] public float ChaseSpeed = 100f;
    [Export] public float ForcePushDamage = 200f;
    [Export] public float Gravity = 20f;
    [Export] public bool IsEnemyThatSearch = false;



    [ExportGroup("Detection")]
    [Export] public PatrolType PatrolStyle = PatrolType.XAxis;

}
