using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy;
using KatMyha.Scripts.Managers;
using KatrinaGame.Core;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Scripts.Utils;
using PrototipoMyha.Utilidades;

namespace KatMyha.Scripts.Enemies.BaseGuardV2
{
    enum StartDirectionEnum
    {
        Left = -1,
        Right = 1
    }
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

        [Export] private StartDirectionEnum StartDirection = StartDirectionEnum.Left;

        private bool hasEmittedKillSignal = false;
        private SignalManager SignalManager;
        private SoundManager SoundManager;

        public override void _Ready()
        {
            base._Ready();
            RaycastUtils.FlipRaycast((int)StartDirection, [RayCast2DDetection]);
            SpriteUtils.FlipSprite((int)StartDirection, AnimatedSprite2DEnemy);
            PolyngUtils.Flip((int)StartDirection, Polygon2DDetection);
            SignalManager = SignalManager.Instance;
            SoundManager = SoundManager.Instance;
        }

        public override void _Process(double delta)
        {
            ProcessKillOfPlayer();
            if (hasEmittedKillSignal)
            {
                GetTree().CreateTimer(1.0).Timeout += () =>
                {
                    hasEmittedKillSignal = false;
                };
            }
        }

        private void ProcessKillOfPlayer()
        {
            if (this.RayCast2DDetection != null)
            {
                (BasePlayer player, bool isColliding) = RaycastUtils.IsColliding<BasePlayer>(this.RayCast2DDetection);
                if (isColliding
                    && !hasEmittedKillSignal)
                {
                    SignalManager.EmitSignal(nameof(SignalManager.EnemyKillMyha));
                    SoundManager.PlaySound(player.DeathAudioStreamPlayer2D);
                    this.Velocity = Vector2.Zero;
                    hasEmittedKillSignal = true;
                    this.Velocity = Vector2.Zero;
                }
            }
        }

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
    }
}
