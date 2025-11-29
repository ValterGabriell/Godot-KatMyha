using Godot;
using Godot.Collections;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Enemy;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Utilidades;
using System;
using System.Linq;

namespace KatMyha.Scripts.Enemies.DroneEnemy.States
{
    public partial class EnemyRoamingState : EnemyStateBase
    {
        private Vector2 _centerPoint;
        private float _time = 0.0f;

        private const float HoverRadius = 40.0f;
        private const float HoverSpeed = 1.2f;

        private const float DriftRadius = 120.0f;
        private const float DriftSpeed = 0.4f;
        private EnemyBaseV2 EnemyBaseV2 => _enemy;
        private DroneEnemy DroneEnemy => _enemy as DroneEnemy;
        private Vector2 TargetPosition = new Vector2();



        public EnemyRoamingState(EnemyBaseV2 enemy, StateMachine stateMachine) : base(enemy, stateMachine)
        {
            this.DroneEnemy.Area2D.BodyEntered += OnBodyEntered;
        }

        public override void EnterState(EnemyStateBase prevState)
        {
            GDLogger.LogRed("Entering EnemyRoamingState");
            base.EnterState(prevState);

            if (_enemy != null)
            {
                this._enemy.SetEnemyState(EnumEnemyState.Roaming);
                _centerPoint = _enemy.GlobalPosition;
                _time = 0.0f;
            }
        }


        public override void Process(float delta)
        {
            if (_enemy == null) return;
            _time += delta;

            var hoverOffset = new Vector2(
                Mathf.Sin(_time * HoverSpeed),
                Mathf.Cos(_time * HoverSpeed)
            ) * HoverRadius;

            var driftOffset = new Vector2(
                Mathf.Sin(_time * DriftSpeed * 0.7f),
                Mathf.Sin(_time * DriftSpeed * 1.3f)
            ) * DriftRadius;

            var target = _centerPoint + driftOffset + hoverOffset;

            var dir = (target - _enemy.GlobalPosition).Normalized();

            _enemy.Velocity = dir * _enemy.Resources.MoveSpeed * delta;
        }

        public override void PhysicsProcess(float delta)
        {
            if (_enemy == null) return;
               
            _enemy.MoveAndSlide();
        }

        private void OnBodyEntered(Node2D body)
        {
            if (body.IsInGroup(EnumGroups.player.ToString()))
            {
                TransitionTo(EnumEnemyState.Chasing);
            }
        }


    }
}
