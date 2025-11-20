using Godot;
using PrototipoMyha.Enemy;
using PrototipoMyha.Enemy.States;

namespace KatMyha.Scripts.Enemies.DroneEnemy.States
{
    public abstract partial class EnemyStateBase : Node
    {
        protected EnemyBaseV2 _enemy;
        protected StateMachine _stateMachine;

        protected EnemyStateBase(EnemyBaseV2 enemy)
        {
            _enemy = enemy;
        }

        public virtual void Enter(EnemyStateBase prevState)
        {
            GD.Print($"[State] Entering {GetType().Name} from {prevState?.GetType().Name ?? "null"}");
        }

      
        public virtual void Exit(EnemyStateBase nextState)
        {
            GD.Print($"[State] Exiting {GetType().Name} to {nextState?.GetType().Name ?? "null"}");
        }


        public virtual void Update(float delta)
        {
        }

        public virtual void PhysicsUpdate(float delta)
        {
        }

        public StateMachine GetStateMachine()
        {
            return _stateMachine;
        }

        protected void TransitionTo(EnemyState newState)
        {
            _stateMachine?.ChangeState(newState);
        }
    }
}