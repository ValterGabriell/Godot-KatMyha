using Godot;
using PrototipoMyha.Enemy;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Utilidades;

namespace KatMyha.Scripts.Enemies.DroneEnemy.States
{
    public partial class EnemyAlertedState : EnemyStateBase
    {
        private Vector2 _lastKnownPosition;
        private float _searchTimer = 0.0f;
        private const float MaxSearchTime = 5.0f;
        private const float SearchRadius = 100.0f;

        public EnemyAlertedState(EnemyBaseV2 enemy) : base(enemy)
        {
        }

        public override void EnterState(EnemyStateBase prevState)
        {
            base.EnterState(prevState);
            
            _lastKnownPosition = PlayerManager.GetPlayerGlobalInstance().GetPlayerPosition();
            _searchTimer = 0.0f;

            if (_enemy != null)
            {
;
            }
        }

        public override void ExitState(EnemyStateBase nextState)
        {
            base.ExitState(nextState);
            
            if (_enemy != null)
            {

            }
        }

        public override void Process(float delta)
        {
            if (_enemy == null) return;

            _searchTimer += delta;

            if (IsPlayerVisible())
            {
                TransitionTo(EnemyState.Chasing);
                return;
            }

            var distanceToTarget = _enemy.GlobalPosition.DistanceTo(_lastKnownPosition);
            
            if (distanceToTarget > 10.0f)
            {
                var direction = (_lastKnownPosition - _enemy.GlobalPosition).Normalized();
     
            }
            else
            {
                var offset = new Vector2(
                    Mathf.Cos(_searchTimer * 2.0f),
                    Mathf.Sin(_searchTimer * 2.0f)
                ) * SearchRadius;
                
                var searchPosition = _lastKnownPosition + offset;
                var searchDirection = (searchPosition - _enemy.GlobalPosition).Normalized();

            }

            if (_searchTimer >= MaxSearchTime)
            {
                TransitionTo(EnemyState.Roaming);
            }
        }

        public override void PhysicsProcess(float delta)
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
