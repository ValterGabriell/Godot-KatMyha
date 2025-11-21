using Godot;
using PrototipoMyha.Enemy;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Utilidades;

namespace KatMyha.Scripts.Enemies.DroneEnemy.States
{
    public abstract partial class EnemyStateBase : Node
    {
        protected EnemyBaseV2 _enemy;
        protected StateMachine _stateMachine;

        protected EnemyStateBase(EnemyBaseV2 enemy, StateMachine stateMachine)
        {
            _enemy = enemy;
            _stateMachine = stateMachine;
        }

        public virtual void EnterState(EnemyStateBase prevState)
        {
            GD.Print($"[State] Entering {GetType().Name} from {prevState?.GetType().Name ?? "null"}");
        }

      
        public virtual void ExitState(EnemyStateBase nextState)
        {
            GD.Print($"[State] Exiting {GetType().Name} to {nextState?.GetType().Name ?? "null"}");
        }

        public override void _PhysicsProcess(double delta)
        {
            Process((float)delta);
        }

        public override void _Process(double delta)
        {
            PhysicsProcess((float)delta);
        }

        public abstract void Process(float delta);

        public abstract void PhysicsProcess(float delta);

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