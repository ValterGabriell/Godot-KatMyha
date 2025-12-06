using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy;
using KatMyha.Scripts.Enemies.DroneEnemy.States;
using KatrinaGame.Scripts.Utils;
using PrototipoMyha.Enemy;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Scripts.Utils;
using PrototipoMyha.Utilidades;
using System;


namespace KatMyha.Scripts.Enemies.BaseGuardV2.States
{
    public partial class BaseGuardStateRoaming : EnemyStateBase
    {
        private Vector2 markerA;
        private Vector2 markerB;
        private Vector2 currentTarget;
        private BaseGuardV2 BaseGuardV2;
        private bool hasValidMarkers;
        private bool IsInitialized;


        public BaseGuardStateRoaming(EnemyBaseV2 enemy, StateMachine stateMachine)
            : base(enemy, stateMachine)
        {
            this.BaseGuardV2 = enemy as BaseGuardV2;
            IsInitialized = false;

            if (this.BaseGuardV2 == null)
            {
                return;
            }

            if (this.BaseGuardV2.RoamMarkerA == null || this.BaseGuardV2.RoamMarkerB == null)
            {
                hasValidMarkers = false;
                return;
            }

            hasValidMarkers = true;
        }

        public override void EnterState(EnemyStateBase previousState)
        {
            base.EnterState(previousState);

            GDLogger.Print($"Entrando no estado Roaming para o inimigo {BaseGuardV2.Name}");
            GDLogger.Print($"Posição atual do inimigo: {BaseGuardV2.GlobalPosition}");
            GDLogger.LogGreen($"Marker A: {BaseGuardV2.RoamMarkerA.GlobalPosition}, Marker B: {BaseGuardV2.RoamMarkerB.GlobalPosition}");
            GDLogger.LogBlue($"Inimigo JustLoaded: {BaseGuardV2.JustLoaded}");
            GDLogger.LogBlue($"Estado IsInitialized: {IsInitialized}");
            GDLogger.Print($"Verificação de markers válidos: {hasValidMarkers}");

            if (!hasValidMarkers)
            {
                TransitionTo(EnumEnemyState.Idle);
                return;
            }

            // Só inicializa os markers na primeira vez
            if (!IsInitialized || !BaseGuardV2.JustLoaded)
            {
                this.markerA = this.BaseGuardV2.RoamMarkerA.GlobalPosition;
                this.markerB = this.BaseGuardV2.RoamMarkerB.GlobalPosition;

                // Define o target inicial baseado na posição atual
                float distToA = BaseGuardV2.GlobalPosition.DistanceTo(markerA);
                float distToB = BaseGuardV2.GlobalPosition.DistanceTo(markerB);
                currentTarget = distToA < distToB ? markerB : markerA;

                IsInitialized = true;

                GD.Print($"Estado Roaming inicializado. MarkerA: {markerA}, MarkerB: {markerB}, Target: {currentTarget}");
            }
            else
            {
                // Se foi carregado de um save, mantém a posição atual e define o próximo target
                float distToA = BaseGuardV2.GlobalPosition.DistanceTo(markerA);
                float distToB = BaseGuardV2.GlobalPosition.DistanceTo(markerB);
                currentTarget = distToA < distToB ? markerB : markerA;

                GD.Print($"Estado Roaming retomado após load. Posição atual: {BaseGuardV2.GlobalPosition}, Target: {currentTarget}");
                BaseGuardV2.JustLoaded = false;
            }
        }

        public override void PhysicsProcess(float delta)
        {
            if (!hasValidMarkers)
            {
                return;
            }

            GDLogger.Print($"Inimigo {BaseGuardV2.Name} movendo-se em direção a {currentTarget}");
            var direction = (currentTarget - BaseGuardV2.GlobalPosition).Normalized();
            var newPos = direction * this.BaseGuardV2.Resources.MoveSpeed * delta;

            BaseGuardV2.GlobalPosition += new Vector2(newPos.X, 0);

            if (BaseGuardV2.GlobalPosition.DistanceTo(currentTarget) < 15.0f)
            {
                currentTarget = currentTarget == markerA ? markerB : markerA;
                TransitionTo(EnumEnemyState.Waiting);
            }

            PlayAnimationRoaming(BaseGuardV2);
            FlipEnemyDirection(BaseGuardV2, direction);
        }



        public override void Process(float delta)
        {
            if (!hasValidMarkers)
            {
                return;
            }

        }

        private static void FlipEnemyDirection(BaseGuardV2 InEnemy, Vector2 direction)
        {
            int directionSign = direction.X > 0 ? 1 : -1;

            RaycastUtils.FlipRaycast(directionSign, [InEnemy.RayCast2DDetection]);
            SpriteUtils.FlipSprite(directionSign, InEnemy.AnimatedSprite2DEnemy);
            PolyngUtils.Flip(directionSign, InEnemy.Polygon2DDetection);
        }

        private static void PlayAnimationRoaming(BaseGuardV2 InEnemy)
        {
            if (InEnemy.AnimatedSprite2DEnemy.Animation != EnumBaseGuardV2_Animation.roaming.ToString())
            {
                InEnemy.AnimatedSprite2DEnemy.Play(EnumBaseGuardV2_Animation.roaming.ToString());
            }
        }

    }
}
