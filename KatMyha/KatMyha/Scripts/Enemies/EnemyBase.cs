using Godot;
using KatMyha.Scripts.Managers;
using KatrinaGame.Core;
using PrototipoMyha.Enemy.Components.Interfaces;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Scripts.Utils.Objetos;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PrototipoMyha.Enemy;

public abstract partial class EnemyBase : CharacterBody2D
{
    [Export] public EnemyResources EnemyResource;

    [ExportGroup("Detection")]
    [Export] public RayCast2D RayCast2DDetection = null;
    [Export] public CircleShape2D CircleAreaDetection = null;
    [Export] public Polygon2D Polygon2DDetection { get; private set; } = null;

    [ExportGroup("Chasing")]
    [Export] public Timer TimerToChase = null;
    [Export] public AnimatedSprite2D AnimatedSprite2DEnemy = null;

    [Export] public float PatrolRadius = 900f;

    [ExportGroup("Bounderies")]
    [Export] public Marker2D Marker_01 = null;
    [Export] public Marker2D Marker_02 = null;
    [Export] public PackedScene BulletShoot;

    public bool JustLoaded { get; set; } = false;
    protected Dictionary<string, IEnemyBaseComponents> Components = new();
    private SoundManager SoundManager = SoundManager.Instance;


    public EnemyState CurrentEnemyState { get; private set; }  = EnemyState.Roaming;
    private Guid Identifier = Guid.NewGuid();
    private bool HasJustSearchSomePlace = false;
    private bool hasEmittedKillSignal = false;

    public Guid GetIdentifier()
    {
        return Identifier;
    }

    public void SetPolygonRoamingColor()
    {
        this.Polygon2DDetection.Color = new Godot.Color(1f, 1f, 1f, 0.5f);
    }

    public void SetPolygonAlertedColor()
    {
        this.Polygon2DDetection.Color = new Godot.Color(1f, 1f, 0f, 0.5f);
    }

    public void SetPolygonDetectionColor()
    {
        this.Polygon2DDetection.Color = new Godot.Color(1f, 0f, 0f, 0.5f);
    }

    public void ProcessKillOfPlayer()
    {
        if (this.RayCast2DDetection != null)
        {
            (BasePlayer player, bool isColliding) = RaycastUtils.IsColliding<BasePlayer>(this.RayCast2DDetection);

            if (isColliding
                && !hasEmittedKillSignal)
            {
                var manager = SignalManager.Instance;
                manager.EmitSignal(nameof(SignalManager.EnemyKillMyha));
                SoundManager.PlaySound(player.DeathAudioStreamPlayer2D);
                this.Velocity = Vector2.Zero;
                hasEmittedKillSignal = true;
            }
        }
    }

    public override void _Ready() 
    {
        InstanciateSpecificComponents();
        TimerToChase.Timeout += OnTimerToChaseTimeout;
        //SignalManager.Instance.EnemyShoot += Shoot;

        foreach (var component in Components.Values)
        {
            component.Initialize();
        }
        if (JustLoaded)
        {
            JustLoaded = false;
            return; //ignora o alerta inicial, foi feito porque ao carregar, ele voltava ao estado de alerta
        }
    }

    private void Shoot(Vector2 positionToGoShoot)
    {
        AnimatedSprite2D sprite = (AnimatedSprite2D)BulletShoot.Instantiate();
        sprite.Position = this.Position;
        AddChild(sprite);

        if (!sprite.IsPlaying())
            sprite.Play("default");

        Tween tween = this.CreateTween();
        tween.TweenProperty(sprite, "position", positionToGoShoot, 0.5f);

        Timer timer = new Timer();
        timer.WaitTime = 2.5f;
        timer.OneShot = true;
        AddChild(timer);

        timer.Timeout += () =>
        {
            if (IsInstanceValid(sprite))
            {
                sprite.QueueFree();
            }
            timer.QueueFree();
        };
    }

    private void OnTimerToStayAlertTimeout()
    {
        if (this.CurrentEnemyState == EnemyState.Alerted)
            SetState(EnemyState.Waiting);
    }


    private void OnTimerToChaseTimeout()
    {
        if(this.CurrentEnemyState == EnemyState.Chasing)
            SetState(EnemyState.Waiting);

    }

    public void SetState(EnemyState newState)
    {
        CurrentEnemyState = newState;
    }


    public void AddComponent<T>(T component) where T : IEnemyBaseComponents
    {
        Components[component.GetType().ToString()] = component;
        AddChild(component as Node);
    }

    public void RemoveComponent<T>(T component) where T : IEnemyBaseComponents
    {
        Components.Remove(component.GetType().ToString());
        RemoveChild(component as Node);
        (component as Node).QueueFree();
    }
    protected abstract void InstanciateSpecificComponents();

    public override void _PhysicsProcess(double delta)
    {
        foreach (var component in Components.Values)
        {
            component.PhysicsProcess(delta);
        }

        if (!IsOnFloor())
        {
    
            this.Velocity += new Vector2(0, EnemyResource.Gravity) * (float)delta;
        }
        MoveAndSlide();
    }

    public override void _Process(double delta)
    {
        foreach (var component in Components.Values)
        {
            component.Process(delta);
        }
    }

    protected EnemyState CurrentState = EnemyState.Roaming;

    public EnemySaveData ToSaveData()
    {
        return new EnemySaveData
        {
            InstanceID = Identifier,
            PositionX = this.GlobalPosition.X,
            PositionY = this.GlobalPosition.Y,
            EnemyState = this.CurrentEnemyState
        };
    }

    public bool GetHasJustSearchSomePlace()
    {
        return this.HasJustSearchSomePlace;
    }

    public void SetInHiddenPlace()
    {
        this.SetState(EnemyState.SearchingHiddenPlace);
        this.HasJustSearchSomePlace = true;
    }
}
