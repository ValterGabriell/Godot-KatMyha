using Godot;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Scripts.Managers;

namespace KatMyha.Scripts.Enemies.DroneEnemy.States
{
    public partial class EnemyKillingState : EnemyStateBase
    {
        private Vector2 _targetPosition;
        private float _timeSinceLastSeen = 0.0f;
        private EnemyBaseV2 EnemyBaseV2 => _enemy;

        public EnemyKillingState(EnemyBaseV2 enemy, StateMachine stateMachine) : base(enemy, stateMachine)
        {
        }

        public override void EnterState(EnemyStateBase prevState)
        {
            if (_enemy != null)
            {
                _enemy.SetEnemyState(EnumEnemyState.Alerted);
                GameManager.GetGameManagerInstance().KillPlayer(EnemyBaseV2);
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
            
        }

        public override void PhysicsProcess(float delta)
        {
            
        }

        
    }
}
