using Godot;
using PrototipoMyha.Enemy;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Utilidades;

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

        public EnemyRoamingState(EnemyBaseV2 enemy) : base(enemy)
        {
        }

        public override void Enter(EnemyStateBase prevState)
        {
            GDLogger.LogRed("Entering EnemyRoamingState");
            base.Enter(prevState);

            if (_enemy != null)
            {
                _centerPoint = _enemy.GlobalPosition;
                _time = 0.0f;
            }
        }

        public override void Update(float delta)
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

        public override void PhysicsUpdate(float delta)
        {
            if (_enemy == null) return;
            
            _enemy.MoveAndSlide();
        }
    }
}