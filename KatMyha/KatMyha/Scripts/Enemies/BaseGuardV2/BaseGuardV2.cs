using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy;
using KatrinaGame.Players;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Player.StateManager;

namespace KatMyha.Scripts.Enemies.BaseGuardV2
{
    public partial class BaseGuardV2 : EnemyBaseV2
    {
        [ExportGroup("Roaming State")]

        /// <summary>
        /// Gets or sets the primary roaming marker used for navigation or positioning.
        ///  <summary>
        [Export]public Marker2D RoamMarkerA { get; private set; } = null;

        /// <summary>
        /// Gets or sets the secondary roaming marker used for navigation or positioning.
        /// </summary>
        [Export] public Marker2D RoamMarkerB { get; private set; } = null;


        [ExportGroup("Detection")]
        /// <summary>
        /// Gets or sets the RayCast2D object used for detection purposes.
        /// </summary>
        [Export] public RayCast2D RayCast2DDetection { get; private set; } = null;

        /// <summary>
        /// Gets the 2D polygon used for detection purposes.
        /// </summary>
        [Export] public Polygon2D Polygon2DDetection { get; private set; } = null;

        [ExportGroup("Sprites")]

        /// <summary>
        /// Gets the animation sprite of a enemy
        /// <summary>
        [Export] public AnimatedSprite2D AnimatedSprite2DEnemy { get; private set; } = null;

        private bool CanSeePlayer()
        {
            if (!RayCast2DDetection.IsColliding())
                return false;

            var collider = RayCast2DDetection.GetCollider();
            if (collider is MyhaPlayer player)
            {
                return player.CurrentPlayerState != PlayerState.HIDDEN;
            }

            return false;
        }

        public void CheckIfHasToChasePlayer()
        {
            if (CanSeePlayer() && this.CurrentEnemyState != EnumEnemyState.Chasing)
            {
                this.EnemyStateBase.TransitionTo(EnumEnemyState.Chasing);
                return;
            }
        }

    }
}
