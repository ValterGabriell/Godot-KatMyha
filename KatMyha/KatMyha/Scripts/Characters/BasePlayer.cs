using Godot;
using KatrinaGame.Core.Interfaces;
using PrototipoMyha;
using PrototipoMyha.Enemy.Components.Interfaces;
using PrototipoMyha.Player.Components.Interfaces;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;

namespace KatrinaGame.Core
{
    public abstract partial class BasePlayer : CharacterBody2D
    {
        [ExportGroup("Player Settings")]
        [Export] public bool IsMovementBlocked { get; private set; } = false;
        [Export] public float Speed { get; set; } = 200f;
        [Export] public float WallWalkSpeed { get; set; } = 10f;
        [Export] public float SneakSpeed { get; set; } = 500f;
        [Export] public float JumpVelocity { get; set; } = -200f;
        [Export] public float Gravity { get; set; } = 700f;
        [Export] public Timer TimeToFallWall { get; set; }
        public PlayerState CurrentPlayerState { get; private set; } = PlayerState.IDLE;
        public LightHiddenState CurrentHiddenState { get; private set; } = LightHiddenState.HIDDEN;
        private StaticBody2D DoorThatEnemyIs {  get; set; } 

        protected Dictionary<string, IPlayerBaseComponent> Components = new();
        [Export] public AnimatedSprite2D AnimatedSprite2D { get; set; }
        [Export] public AnimatedSprite2D SoundAnimatedSprite2D { get; set; }


        public override void _Ready()
        {
            InstanciateComponents();
            foreach (var component in Components.Values)
            {
                component.Initialize(this);
            }
        }

        public void BlockMovement()
        {
            IsMovementBlocked = true;
        }

        public void UnblockMovement()
        {
            IsMovementBlocked = false;
        }

        public override void _Process(double delta)
        {
            foreach (var component in Components.Values)
            {
                component.Process(delta);
            }
        }

        public void SetIsEnemyOnDoor(StaticBody2D door)
        {
            DoorThatEnemyIs = door;
            var isLocked = (bool)door.Get("is_locked");
            var requiredKeyName = (string)door.Get("required_key_name");

            // Você pode usar essas variáveis conforme necessário
            GD.Print($"A porta está trancada? {isLocked}, chave necessária: {requiredKeyName}");
        }
        public void RemoveEnemyOnDoor() => DoorThatEnemyIs = null;
        public StaticBody2D GetDoorThatEnemyIs() => DoorThatEnemyIs;
        


        public override void _PhysicsProcess(double delta)
        {
            HandleInput(delta);

            foreach (var component in Components.Values)
            {
                component.PhysicsProcess(delta);
            }
            if (IsMovementBlocked && Velocity.Length() > 0f)
            {
                Velocity *= 0.8f;
                if (Velocity.Length() < 1f)
                    Velocity = Vector2.Zero;
            }
            MoveAndSlide();
        }

        public void SetState(PlayerState newState)
        {
            CurrentPlayerState = newState;
        }

        public void SetStateHidden(LightHiddenState newState)
        {
            CurrentHiddenState = newState;
        }


        public PlayerState GetState()
        {
            return this.CurrentPlayerState;
        }

        protected void FlipRaycast(float direction, List<RayCast2D> Rasycast)
        {
            foreach (var current in Rasycast)
            {
                current.TargetPosition = new Vector2(direction * Mathf.Abs(current.TargetPosition.X), current.TargetPosition.Y);
            }
        }

        protected virtual void HandleInput(double delta)
        {
            foreach (var component in Components.Values)
            {
                component.HandleInput(delta);
            }
        }

        protected abstract void InstanciateComponents();


        public void AddComponent<T>(T component) where T : IPlayerBaseComponent
        {
            Components[typeof(T).ToString()] = component;
            AddChild(component as Node);
            
        }

        public T GetComponent<T>() where T : IPlayerBaseComponent
        {
            Components.TryGetValue(typeof(T).ToString(), out var component);
            return (T)component;
        }

    }
}