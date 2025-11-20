using Godot;
using PrototipoMyha.Enemy;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Utilidades;

namespace KatMyha.Scripts.Enemies.DroneEnemy.States
{
    public partial class EnemyChasingState : EnemyStateBase
    {
        private Vector2 _targetPosition;
        private const float ChaseStoppingDistance = 50.0f;
        private const float LoseTargetTime = 3.0f;
        private float _timeSinceLastSeen = 0.0f;

        public EnemyChasingState(EnemyBaseV2 enemy) : base(enemy)
        {
        }

        public override void Enter(EnemyStateBase prevState)
        {
            base.Enter(prevState);
            _timeSinceLastSeen = 0.0f;
            
            if (_enemy != null)
            {
          
            }
        }

        public override void Exit(EnemyStateBase nextState)
        {
            base.Exit(nextState);
            
            if (_enemy != null)
            {
       
            }
        }

        public override void Update(float delta)
        {
            if (_enemy == null) return;

            _targetPosition = PlayerManager.GetPlayerGlobalInstance().GetPlayerPosition();

            var distanceToPlayer = _enemy.GlobalPosition.DistanceTo(_targetPosition);

            if (IsPlayerVisible())
            {
                _timeSinceLastSeen = 0.0f;
                
                if (distanceToPlayer > ChaseStoppingDistance)
                {
                    var direction = (_targetPosition - _enemy.GlobalPosition).Normalized();
  
                }
                else
                {
                    _enemy.Velocity = Vector2.Zero;
                }
            }
            else
            {
                _timeSinceLastSeen += delta;

                if (_timeSinceLastSeen >= LoseTargetTime)
                {
                    TransitionTo(EnemyState.Alerted);
                }
            }
        }

        public override void PhysicsUpdate(float delta)
        {
            if (_enemy == null) return;
            
            _enemy.MoveAndSlide();
        }

        private bool IsPlayerVisible()
        {
            return false;
        }
    }
}
