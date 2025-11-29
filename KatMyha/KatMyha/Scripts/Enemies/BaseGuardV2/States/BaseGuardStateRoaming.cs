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


        public BaseGuardStateRoaming(EnemyBaseV2 enemy, StateMachine stateMachine)
            : base(enemy, stateMachine)
        {
            this.BaseGuardV2 = enemy as BaseGuardV2;
            this.markerA = this.BaseGuardV2.RoamMarkerA.GlobalPosition;
            this.markerB = this.BaseGuardV2.RoamMarkerB.GlobalPosition;
            currentTarget = markerB;
        }


        public override void PhysicsProcess(float delta)
        {
            var direction = (currentTarget - BaseGuardV2.Position).Normalized();
            var newPos = direction * this.BaseGuardV2.Resources.MoveSpeed * delta;

            BaseGuardV2.Position += new Vector2(newPos.X, 0);

            if (BaseGuardV2.Position.DistanceTo(currentTarget) < 15.0f)
            {
                currentTarget = currentTarget == markerA ? markerB : markerA;
                TransitionTo(EnumEnemyState.Waiting);
            }

            PlayAnimationRoaming(BaseGuardV2);
            FlipEnemyDirection(BaseGuardV2, direction);
        }



        public override void Process(float delta){
            this.BaseGuardV2.CheckIfHasToChasePlayer();
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
