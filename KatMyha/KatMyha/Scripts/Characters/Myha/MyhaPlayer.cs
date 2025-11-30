using Godot;
using KatMyha.Scripts.Characters.Myha.Components.Impl;
using KatrinaGame.Components;
using KatrinaGame.Core;
using KatrinaGame.Core.Interfaces;
using PrototipoMyha;
using PrototipoMyha.Player.Components.Impl;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Scripts.Characters.Myha.Components.Impl;
using PrototipoMyha.Utilidades;

namespace KatrinaGame.Players
{
    public partial class MyhaPlayer : BasePlayer
    {
        [ExportGroup("RayCasts")]
        [Export] public RayCast2D AttackRaycast { get; set; }

        [ExportGroup("Scenes")]
        [Export] public PackedScene BallScene { get; set; }

        [ExportGroup("Areas2D")]
        [Export] public Area2D SoundAreaWalkingComponent { get; set; }
        [Export] public Area2D WallJumpArea { get; set; }


        [ExportGroup("AudioStreams")]
        [Export] public AudioStreamPlayer2D WalkAudioStreamPlayer2D { get; set; }
        [Export] public AudioStreamPlayer2D JumpAudioStreamPlayer2D { get; set; }
        [Export] public AudioStreamPlayer2D SaveAudioStreamPlayer2D { get; set; }

        [ExportGroup("CollisionShapes")]
        private CircleShape2D SoundAreaWalkingColiisonComponent { get; set; }

        public Camera2D InitialCameraSettings { get; private set; }


        [ExportGroup("UI")]
        [Export] public UIGame UICodigo { get; set; }

        private IMovementComponent MovementComponent;
        private float CurrentPlayerSpeed = 0f;




        /*WALL JUMP*/
        private int WallDirection { get; set; } = 0;
        [Export] public Vector2 WallJumpForce { get; set; } = new Vector2(250, -400);


        protected override void InstanciateComponents()
        {
            AddComponent<IMovementComponent>(new MovementComponent());
            AddComponent<IMakeSoundWhileWalkComponent>(new MakeSoundWhileWalkComponent(this));
            AddComponent<IAnimationComponents>(new AnimationComponents(this));
            AddComponent<IShootAimComponent>(new ShootAimComponent());
            AddComponent<IToogleLightComponent>(new ToogleLightComponent());
            InitialCameraSettings = this.GetNode<Camera2D>("Camera2D");

            MovementComponent = GetComponent<IMovementComponent>();
            SubscribeSignals();
            SoundAreaWalkingColiisonComponent = SoundAreaWalkingComponent
            .GetNode<CollisionShape2D>("CollisionShape2D")
            .Shape as CircleShape2D;
        }

        private void SubscribeSignals()
        {
            SignalManager.Instance.PlayerStateChanged += PlayerStateChanged;
            SignalManager.Instance.GameLoaded += UpdatePlayerPosition;


        }


        public void _on_wall_jump_area_body_entered(Node2D node2D)
        {
            if (node2D.IsInGroup("wall_vertical"))
            {
                this.SetState(PlayerState.WALL_SLIDING);
            }
        }

        public void _on_wall_jump_area_body_exited(Node2D node2D)
        {
            if (node2D.IsInGroup("wall_vertical") && this.CurrentPlayerState == PlayerState.WALL_SLIDING)
            {
                this.SetState(PlayerState.FALLING);
            }
        }

        public void _on_light_detection_area_entered(Area2D node)
        {
            if (node.IsInGroup("kill_light"))
            {
                this.SetStateHidden(MyhaContactLightHiddenState.MYHA_IS_ON_LIGHT);
                SignalManager.Instance.EmitSignal(nameof(SignalManager.Instance.PlayerIsOnLight));
            }
        }

        public void _on_light_detection_area_exited(Area2D node)
        {
            if (node.IsInGroup("kill_light"))
            {
                this.SetStateHidden(MyhaContactLightHiddenState.MYHA_IS_NOT_ON_LIGHT);

            }
        }

        private void UpdatePlayerPosition(Vector2 position)
        {
            this.GlobalPosition = position;
        }

        public void AlterRadiusCollisionSoundArea(float newRadius)
        {
            SoundAreaWalkingColiisonComponent.Radius = newRadius;
        }

        private void PlayerStateChanged(PlayerState NewState)
        {
            this.SetState(NewState);
        }


        protected override void HandleInput(double delta)
        {
            // Input de movimento
            Vector2 inputVector = Vector2.Zero;
            if (inputVector == Vector2.Zero) CurrentPlayerSpeed = 0f;


            if (this.CurrentPlayerState != PlayerState.WALL_SLIDING)
            {
                if (Input.IsActionPressed("d") && this.IsMovementBlocked == false && this.CurrentPlayerState != PlayerState.AIMING)
                {
                    inputVector.X += 1;
                    FlipRaycast(direction: 1,
                    [
                        AttackRaycast
                    ]);
                    CurrentPlayerSpeed = Speed;
                }

                if (Input.IsActionPressed("a") && this.IsMovementBlocked == false && this.CurrentPlayerState != PlayerState.AIMING)
                {
                    inputVector.X -= 1;
                    FlipRaycast(direction: -1,
                   [
                       AttackRaycast
                   ]);
                    CurrentPlayerSpeed = Speed;
                }


                if (Input.IsKeyPressed(Key.Ctrl) && this.CurrentPlayerState != PlayerState.AIMING)
                {

                    CurrentPlayerSpeed = SneakSpeed;
                }
            }

            if (this.CurrentPlayerState == PlayerState.WALL_SLIDING)
            {
                if (Input.IsActionPressed("w") && this.IsMovementBlocked == false)
                {
                    inputVector.Y -= 1;
                    CurrentPlayerSpeed = WallWalkSpeed;
                }

                if (Input.IsActionPressed("s") && this.IsMovementBlocked == false)
                {
                    inputVector.Y += 1;
                    CurrentPlayerSpeed = WallWalkSpeed;
                }
            }


            if (Input.IsActionJustPressed("jump") && this.CurrentPlayerState != PlayerState.AIMING)
            {
                inputVector.Y -= 1;
                MovementComponent.Jump();
            }


            if (Input.IsActionJustPressed("action"))
            {
                ProcessActionKey();
            }

            if (Input.IsActionJustPressed(nameof(EnumActionsInput.shoot_options_toogle)))
            {
                if (PlayerManager.GetPlayerGlobalInstance().GetCurrentPlayerShootType() == PlayerShootType.AIM_SHOOT)
                    PlayerManager.GetPlayerGlobalInstance().SetCurrentPlayerShootType(PlayerShootType.DISTRACTION_SHOOT);
                else
                    PlayerManager.GetPlayerGlobalInstance().SetCurrentPlayerShootType(PlayerShootType.AIM_SHOOT);
                GDLogger.LogYellow("Toggled Shoot Type to: " + PlayerManager.GetPlayerGlobalInstance().GetCurrentPlayerShootType().ToString());
            }

            MovementComponent.Move(inputVector, CurrentPlayerSpeed);

            base.HandleInput(delta);
        }

        private void ProcessActionKey()
        {
            if (this.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_OPEN_DOOR
                    && this.GetDoorThatEnemyIs() != null)
            {
                var door = this.GetDoorThatEnemyIs();
                var isLocked = (bool)door.Get("is_locked");
                var requiredKeyName = (string)door.Get("required_key_name");
                door.Call("try_unlock");
            }

            if (this.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_OUT_HIDDEN_PLACE)
            {
                SetCurrentEnabledAction(PlayerCurrentEnabledAction.NONE);
                CallDeferred(nameof(OutHiddenPlace));
            }

            if (this.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_HIDE)
            {
                CallDeferred(nameof(SetHiddenPlace));
            }

    
        }

        private void SetHiddenPlace()
        {
            UICodigo.EmitSignal("UIChangeVisibility", false);
            this.RemoveMaskOfEnemy();
            this.BlockMovement();
            this.SetState(PlayerState.HIDDEN);
            SetCurrentEnabledAction(PlayerCurrentEnabledAction.CAN_OUT_HIDDEN_PLACE);
            this.ZIndex = -1;
            this.SoundAreaWalkingComponent.Monitoring = false;
        }

        private void OutHiddenPlace()
        {
            UICodigo.EmitSignal("UIChangeVisibility", true);
            this.UnblockMovement();
            this.SetState(PlayerState.IDLE);
            this.ZIndex = 100;
            this.SoundAreaWalkingComponent.Monitoring = true;
        }

        internal void EnterHiddenPlace()
        {
            this.SetCurrentEnabledAction(PlayerCurrentEnabledAction.CAN_HIDE);
        }
    }
}


enum EnumActionsInput
{
    shoot_options_toogle,
    action,
    jump,
    s,
    w,
    a,
    d
}
