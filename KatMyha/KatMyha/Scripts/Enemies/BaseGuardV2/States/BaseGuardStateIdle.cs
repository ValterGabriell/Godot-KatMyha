using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy;
using KatMyha.Scripts.Enemies.DroneEnemy.States;
using PrototipoMyha.Scripts.Utils;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatMyha.Scripts.Enemies.BaseGuardV2.States
{
    public partial class BaseGuardStateIdle : EnemyStateBase
    {
        private Vector2 InitialPosition;
        private Vector2 PatrolPosition;
        private Vector2 TargetPosition;
        private BaseGuardV2 BaseGuardV2;
        private float IdleTimer;
        private float IdleWaitTime;
        private bool IsMoving;
        private bool MovingToPatrol;
        private Random Random;
        private const float PatrolDistance = 100.0f;
        private bool IsInitialized;

        public BaseGuardStateIdle(EnemyBaseV2 enemy, StateMachine stateMachine) : base(enemy, stateMachine)
        {
            this.BaseGuardV2 = enemy as BaseGuardV2;
            enemy.Velocity = Vector2.Zero;
            Random = new Random();
            IsInitialized = false;
        }

        public override void EnterState(EnemyStateBase previousState)
        {
            base.EnterState(previousState);

            // Só inicializa a posição na primeira vez ou se não foi carregado de um save
            if (!IsInitialized || !BaseGuardV2.JustLoaded)
            {
                InitialPosition = BaseGuardV2.GlobalPosition;
                PatrolPosition = InitialPosition + new Vector2(PatrolDistance, 0);
                TargetPosition = InitialPosition;
                IdleTimer = 0f;
                IdleWaitTime = GetRandomWaitTime();
                IsMoving = false;
                MovingToPatrol = true;
                IsInitialized = true;

                GD.Print($"Estado Idle inicializado. Posição inicial: {InitialPosition}");
            }
            else
            {
                // Se foi carregado, mantém a posição e recalcula os alvos baseado nela
                InitialPosition = BaseGuardV2.GlobalPosition;
                PatrolPosition = InitialPosition + new Vector2(PatrolDistance, 0);

                // Determina se está indo para patrulha ou voltando baseado na posição atual
                float distToInitial = BaseGuardV2.GlobalPosition.DistanceTo(InitialPosition);
                float distToPatrol = BaseGuardV2.GlobalPosition.DistanceTo(PatrolPosition);

                MovingToPatrol = distToPatrol > distToInitial;
                TargetPosition = MovingToPatrol ? PatrolPosition : InitialPosition;

                IdleTimer = 0f;
                IdleWaitTime = GetRandomWaitTime();
                IsMoving = false;

                GD.Print($"Estado Idle retomado após load. Posição: {InitialPosition}");
                BaseGuardV2.JustLoaded = false;
            }
        }

        public override void PhysicsProcess(float delta)
        {
            if (!IsMoving)
            {
                IdleTimer += delta;
                if (IdleTimer >= IdleWaitTime)
                {
                    IsMoving = true;
                    IdleTimer = 0f;
                    TargetPosition = MovingToPatrol ? PatrolPosition : InitialPosition;
                }
            }
            else
            {
                if (BaseGuardV2.GlobalPosition.DistanceTo(TargetPosition) > 5.0f)
                {
                    Vector2 direction = (TargetPosition - BaseGuardV2.GlobalPosition).Normalized();
                    var newPos = direction * this.BaseGuardV2.Resources.MoveSpeed * delta;
                    BaseGuardV2.GlobalPosition += new Vector2(newPos.X, 0);
                    FlipEnemyDirection(BaseGuardV2, direction);
                }
                else
                {
                    BaseGuardV2.GlobalPosition = TargetPosition;
                    IsMoving = false;
                    MovingToPatrol = !MovingToPatrol;
                    IdleWaitTime = GetRandomWaitTime();
                }
            }
        }

        public override void Process(float delta)
        {

        }

        private float GetRandomWaitTime()
        {
            return (float)(Random.NextDouble() * (10.0 - 8.0) + 8.0);
        }

        private static void FlipEnemyDirection(BaseGuardV2 InEnemy, Vector2 direction)
        {
            int directionSign = direction.X > 0 ? 1 : -1;

            RaycastUtils.FlipRaycast(directionSign, [InEnemy.RayCast2DDetection]);
            SpriteUtils.FlipSprite(directionSign, InEnemy.AnimatedSprite2DEnemy);
            PolyngUtils.Flip(directionSign, InEnemy.Polygon2DDetection);
        }
    }
}
