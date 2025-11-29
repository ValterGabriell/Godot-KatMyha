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
        protected EnemyStateBase LastEnemyState { get; private set; }

        protected EnemyStateBase(EnemyBaseV2 enemy, StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            this._enemy = enemy;
        }

        public virtual void EnterState(EnemyStateBase prevState)
        {
            
        }

      
        public virtual void ExitState(EnemyStateBase nextState)
        {
            
        }

        public override void _PhysicsProcess(double delta)
        {
            PhysicsProcess((float)delta);
          
        }

        public override void _Process(double delta)
        {
            Process((float)delta);
        }


        public abstract void Process(float delta);

        public abstract void PhysicsProcess(float delta);

        public StateMachine GetStateMachine()
        {
            return _stateMachine;
        }

        public void TransitionTo(EnumEnemyState newState)
        {
            LastEnemyState = this;
            _stateMachine?.ChangeState(newState);
            this._enemy.SetEnemyState(newState);
        }

    }
}
