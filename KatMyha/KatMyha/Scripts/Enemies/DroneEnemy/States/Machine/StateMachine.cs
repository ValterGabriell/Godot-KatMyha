using Godot;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Utilidades;
using System.Collections.Generic;

namespace KatMyha.Scripts.Enemies.DroneEnemy.States
{
    public abstract partial class StateMachine : Node
    {
        [Export]
        public EnemyState InitialState { get; set; } = EnemyState.Roaming;
        public EnemyState CurrentStateType { get; private set; }
        
        private EnemyStateBase _currentState;
        private Dictionary<EnemyState, EnemyStateBase> _states = new();
        private EnemyBaseV2 _enemy;

        public override void _Ready()
        {
            _enemy = GetParent<EnemyBaseV2>();
            if (_enemy == null)
            {
                GD.PushError("[StateMachine] Parent must be an EnemyBase!");
                return;
            }

            InitializeStates(_enemy);
            
            if (_states.Count == 0)
            {
                GD.PushWarning("[StateMachine] No states registered!");
                return;
            }

            ChangeState(InitialState);
            _currentState = _states[InitialState];
        }

        protected abstract void InitializeStates(EnemyBaseV2 _enemy);
        public void RegisterState(EnemyState stateType, EnemyStateBase state)
        {
            if (state == null)
            {
                GD.PushError($"[StateMachine] Cannot register null state for {stateType}");
                return;
            }

            if (_states.ContainsKey(stateType))
            {
                GD.PushWarning($"[StateMachine] State {stateType} already registered, replacing...");
            }

            _states.Add(stateType, state);
        }

        public void ChangeState(EnemyState newStateType)
        {
            if (!_states.TryGetValue(newStateType, out var nextState))
            {
                GD.PushError($"[StateMachine] State {newStateType} not found in registered states!");
                return;
            }
            _currentState?.ExitState(nextState);

            var previousState = _currentState;
            
            _currentState = nextState;
            CurrentStateType = newStateType;

            _currentState.EnterState(previousState);
        }

        public override void _Process(double delta)
        {
            _currentState?.Process((float)delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            _currentState?.PhysicsProcess((float)delta);
        }

        public EnemyStateBase GetCurrentState()
        {
            return _currentState;
        }

        public bool HasState(EnemyState stateType)
        {
            return _states.ContainsKey(stateType);
        }

        public IEnumerable<EnemyState> GetRegisteredStates()
        {
            return _states.Keys;
        }
    }
}