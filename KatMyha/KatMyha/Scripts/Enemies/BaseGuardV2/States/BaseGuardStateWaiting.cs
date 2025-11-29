using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy;
using KatMyha.Scripts.Enemies.DroneEnemy.States;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Scripts.Utils;
using PrototipoMyha.Utilidades;
using System;

namespace KatMyha.Scripts.Enemies.BaseGuardV2.States
{
    public partial class BaseGuardStateWaiting : EnemyStateBase
    {

        private float _waitingTime = 2.0f;
        private BaseGuardV2 BaseGuardV2 { get; set; }


        public BaseGuardStateWaiting(EnemyBaseV2 enemy, StateMachine stateMachine) : base(enemy, stateMachine)
        {
            this.BaseGuardV2 = enemy as BaseGuardV2;
            enemy.Velocity = Vector2.Zero;
        }



        public override void PhysicsProcess(float delta)
        {
            _waitingTime -= delta;
            PlayAnimationWaiting(BaseGuardV2);
            if (_waitingTime <= 0.0f)
            {
                TransitionTo(EnumEnemyState.Roaming);
            }
        }

        public override void Process(float delta)
        {
            this.BaseGuardV2.CheckIfHasToChasePlayer();
        }

        private void PlayAnimationWaiting(BaseGuardV2 InEnemy)
        {
            if (InEnemy.AnimatedSprite2DEnemy.Animation != EnumBaseGuardV2_Animation.waiting.ToString())
            {
                InEnemy.AnimatedSprite2DEnemy.Play(EnumBaseGuardV2_Animation.waiting.ToString());
            }
        }

    }
}
