using Godot;
using KatMyha.Scripts.Enemies.BaseGuard.Components.Impl.EnemyMovement.Strategies.StatesHandler;
using PrototipoMyha.Enemy.Components.Impl.EnemyMovement.Strategies.PatrolHandler;
using PrototipoMyha.Enemy.Components.Impl.EnemyMovement.Strategies.StatesHandler;
using PrototipoMyha.Enemy.Components.Impl.EnemyMovement.Strategies.StatesHandler.Chase_Alerted;
using PrototipoMyha.Enemy.Components.Interfaces;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Scripts.Enemies.BaseGuard.Components.Impl.EnemyMovement.Strategies.Interfaces;
using PrototipoMyha.Utilidades;
using System;
using System.Linq;

namespace PrototipoMyha.Enemy.Components.Impl
{
    public partial class EnemyMovementComponent : Node, IEnemyBaseComponents
    {
        private EnemyBase _Enemy;
        private Random _random = new Random();
        private Vector2 _targetPosition;
        private float _waitTimer = 0f;
        private float _maxWaitTime = 3f;

        private Vector2 _initialPosition;
        private EnemyState? LastEnemyState = null;
        private IEnemyStateHandler enemyStateHandler;
        private bool HasCollidedWithBounderie = false;
        private SignalManager SignalManager = SignalManager.Instance;


        public EnemyMovementComponent(EnemyBase enemy)
        {
            this._Enemy = enemy;
            this._initialPosition = enemy.GlobalPosition;
            SignalManager.PlayerIsOnLight += PlayerIsOnLight;
        }

        private void PlayerIsOnLight()
        {
            var playerPosition = PlayerManager.GetPlayerGlobalInstance().GetPlayerPosition();
            foreach (EnemyBase enemy in GetTree().GetNodesInGroup("enemy").OfType<EnemyBase>())
            {
                float distanceToPlayer = enemy.GlobalPosition.DistanceTo(playerPosition);
                if (distanceToPlayer < 200f && enemy.CurrentEnemyState == EnemyState.Roaming)
                {
                    enemy.SetState(EnemyState.Alerted);
                }
            }
        }

        public void Initialize()
        {
            SetNewRandomTarget();
        }


        public void PhysicsProcess(double delta) { }

        public void Process(double delta)
        {
            if (HasEnemyStateChanged())
            {
                LastEnemyState = this._Enemy.CurrentEnemyState;
                enemyStateHandler = GetEnemyStateHandler.GetStateHandler(
                    state: _Enemy.CurrentEnemyState,
                    WaitTime: _waitTimer,
                    MaxWaitTime: _maxWaitTime,
                    SetNewWaitTimeWhenWaiting: SetNewRandomTarget
                );
            }

            if (enemyStateHandler != null)
            {
                // Só passa _targetPosition para estados que precisam (Roaming)
                Vector2? positionToPass = _Enemy.CurrentEnemyState == EnemyState.Roaming 
                    ? _targetPosition 
                    : null;
                
                if(this._Enemy.EnemyResource.EnemyJustMoveWhenDistracted 
                    && (enemyStateHandler is EnemyStateAlertedHandler 
                    || enemyStateHandler is EnemyStateWaitingHandler
                    || enemyStateHandler is EnemyStateStoppedWhileNotDistracted))
                {
                    _waitTimer = enemyStateHandler.ExecuteState(
                        delta: delta,
                        InEnemy: _Enemy,
                        InTargetPosition: positionToPass);
                }

                if (!this._Enemy.EnemyResource.EnemyJustMoveWhenDistracted)
                {
                    _waitTimer = enemyStateHandler.ExecuteState(
                     delta: delta,
                     InEnemy: _Enemy,
                     InTargetPosition: positionToPass);
                }
     
            }
        }



        private bool HasEnemyStateChanged()
        {
            return LastEnemyState != this._Enemy.CurrentEnemyState || LastEnemyState == null;
        }


        private void SetNewRandomTarget()
        {
            IPatrolTypeHandler patrolStrategyHandler = GetPatrolType.GetHandler(this._Enemy.EnemyResource.PatrolStyle);
            Vector2 randomOffset = patrolStrategyHandler.GetPatrolTarget(this._Enemy.PatrolRadius, _random);

            _targetPosition = _initialPosition + randomOffset;

            // Aplicar os limites dos marcadores
            if (_Enemy.Marker_01 != null && _Enemy.Marker_02 != null)
            {
                float minX = Mathf.Min(_Enemy.Marker_01.GlobalPosition.X, _Enemy.Marker_02.GlobalPosition.X);
                float maxX = Mathf.Max(_Enemy.Marker_01.GlobalPosition.X, _Enemy.Marker_02.GlobalPosition.X);

                _targetPosition.X = Mathf.Clamp(_targetPosition.X, minX, maxX);
            }
        }
    }
}
