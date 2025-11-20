using Godot;
using KatrinaGame.Core;
using PrototipoMyha.Utilidades;

namespace PrototipoMyha.Enemy.Components.Impl.EnemyMovement.Strategies.StatesHandler.Chase_Alerted
{
    public class EnemyStateAlertedHandler : EnemyStateChaseAlertedBase
    {
        private float alertWaitDuration = 2.0f;
        private SignalManager SignalManager;
        private Vector2 lastKnownPlayerPosition;
        private bool hasEmittedAlert = false; 
        public EnemyStateAlertedHandler(Vector2 lastKnownPlayerPosition) : base(lastKnownPlayerPosition)
        {
            SignalManager = SignalManager.Instance;
            this.lastKnownPlayerPosition = lastKnownPlayerPosition;
        }

        public override float ExecuteState(
            double delta,
            EnemyBase InEnemy, Vector2? InPositionToChase = null)
        {
            InEnemy.SetPolygonAlertedColor();
            if (!hasEmittedAlert)
            {
                SignalManager.EmitSignal(nameof(SignalManager.EnemySpottedPlayerShowAlert), lastKnownPlayerPosition);
                hasEmittedAlert = true;
            }
         

            (BasePlayer _, bool isColliding) = RaycastUtils.IsColliding<BasePlayer>(InEnemy.RayCast2DDetection);
            if (isColliding)
            {
               InEnemy.SetState(States.EnemyState.Chasing);
               return 0.1f;
            }

            alertWaitDuration -= (float)delta;
            InEnemy.Velocity = Vector2.Zero;
            float waitTime = 3;
            if (alertWaitDuration <= 0f)
            {
                SignalManager.EmitSignal(nameof(SignalManager.EnemySpottedPlayer));

                waitTime = base.ExecuteState(delta, InEnemy);
            }
            return waitTime;
        }
    }
}
