using Godot;
using KatrinaGame.Players;
using PrototipoMyha.Enemy;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Utilidades;
using System;

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
        private RayCast2D SearchPlayerRaycast => _enemy.GetNode<RayCast2D>("SearchPlayerRaycast");
    

        public EnemyRoamingState(EnemyBaseV2 enemy) : base(enemy)
        {
        }

        public override void EnterState(EnemyStateBase prevState)
        {
            GDLogger.LogRed("Entering EnemyRoamingState");
            base.EnterState(prevState);

            if (_enemy != null)
            {
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
            if (this.SearchPlayerRaycast != null)
                PP_LookAtPlayer();
            
            
        }

        private void PP_LookAtPlayer()
        {
            Vector2 playerGlobalPos = PlayerManager.GetPlayerGlobalInstance().GetPlayerPosition();
            SearchPlayerRaycast.TargetPosition = SearchPlayerRaycast.ToLocal(playerGlobalPos);
        }
    }
}