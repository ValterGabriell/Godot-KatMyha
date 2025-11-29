using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy;
using KatMyha.Scripts.Enemies.DroneEnemy.States;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Scripts.Utils;
using PrototipoMyha.Utilidades;

namespace KatMyha.Scripts.Enemies.BaseGuardV2.States
{
    public partial class BaseGuardStateAlertedBySound : EnemyStateBase
    {
        private BaseGuardV2 BaseGuardV2 { get; set; }
        private PlayerManager _PlayerManager = PlayerManager.GetPlayerGlobalInstance();
        private float _TimeSurprisedBySound = 2f;
        private float _TimeInvestigating = 3f;
        public BaseGuardStateAlertedBySound(EnemyBaseV2 enemy, StateMachine stateMachine) : base(enemy, stateMachine)
        {
            this.BaseGuardV2 = enemy as BaseGuardV2;
        }

        public override void PhysicsProcess(float delta)
        {
            _TimeSurprisedBySound -= delta;
            if (_TimeSurprisedBySound <= 0.0f)
            {
                Vector2 targetPos = MoveTowardsSoundSource(delta);
                if (BaseGuardV2.Position.DistanceTo(targetPos) < 15.0f)
                    ProcessSoundInvestigation(delta);
            }
        }

        private void ProcessSoundInvestigation(float delta)
        {
            _TimeInvestigating -= delta;
            if (_TimeInvestigating <= 0.0f)
            {
                TransitionTo(EnumEnemyState.Waiting);
            }
        }

        private Vector2 MoveTowardsSoundSource(float delta)
        {
            Vector2 targetPos = _PlayerManager.LastPlayerPositionThatMakedSound;
            Vector2 direction = (targetPos - BaseGuardV2.GlobalPosition).Normalized();
            BaseGuardV2.GlobalPosition += direction * this.BaseGuardV2.Resources.MoveSpeed * delta;
            FlipEnemyDirection(BaseGuardV2, direction);
            return targetPos;
        }

        public override void Process(float delta) { this.BaseGuardV2.CheckIfHasToChasePlayer(); }

        private static void PlayAnimationAlerted(BaseGuardV2 InEnemy)
        {
            if (InEnemy.AnimatedSprite2DEnemy.Animation != EnumBaseGuardV2_Animation.roaming.ToString())
            {
                InEnemy.AnimatedSprite2DEnemy.Play(EnumBaseGuardV2_Animation.roaming.ToString());
            }
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
