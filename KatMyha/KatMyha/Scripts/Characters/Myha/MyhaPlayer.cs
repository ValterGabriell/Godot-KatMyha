using Godot;
using KatMyha.Scripts.Characters.Myha.Components.Impl;
using KatrinaGame.Components;
using KatrinaGame.Core;
using KatrinaGame.Core.Interfaces;
using KatrinaGame.Scripts.Utils;
using PrototipoMyha;
using PrototipoMyha.Player.Components.Impl;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Scripts.Characters.Myha.Components.Impl;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;

namespace KatrinaGame.Players
{
    public partial class MyhaPlayer : BasePlayer
    {
        [Export] private Camera2D CameraMap { get; set; }
        [Export] private Camera2D CameraPlayer { get; set; }
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
        [Export] public CollisionShape2D HeadCollision { get; set; }

        private CircleShape2D SoundAreaWalkingColiisonComponent { get; set; }



        /*camera*/
        public bool CanMoveCamera { get; private set; } = false;


        [ExportGroup("UI")]
        [Export] public UIGame UICodigo { get; set; }

        private IMovementComponent MovementComponent;
        private float CurrentPlayerSpeed = 0f;
        private PlayerManager PlayerManager;

        //delegate for change hability
        private Dictionary<PlayerHabilityKey, Action> PlayerHabilityMethodToExecute;


        /*WALL JUMP*/
        private int WallDirection { get; set; } = 0;
        [Export] public Vector2 WallJumpForce { get; set; } = new Vector2(250, -400);
        public bool PlayeCanMoveCamera { get; private set; }

        protected override void InstanciateComponents()
        {
            this.CameraPlayer.MakeCurrent();
            AddComponent<IMovementComponent>(new MovementComponent());
            AddComponent<IMakeSoundWhileWalkComponent>(new MakeSoundWhileWalkComponent(this));
            AddComponent<IAnimationComponents>(new AnimationComponents(this));
            AddComponent<IShootAimComponent>(new ShootAimComponent());
            AddComponent<IToogleLightComponent>(new ToogleLightComponent());

            MovementComponent = GetComponent<IMovementComponent>();
            SubscribeSignals();
            SoundAreaWalkingColiisonComponent = SoundAreaWalkingComponent
            .GetNode<CollisionShape2D>("CollisionShape2D")
            .Shape as CircleShape2D;

            PlayerManager = PlayerManager.GetPlayerGlobalInstance();

            /*habilities*/
            PlayerHabilityMethodToExecute = new Dictionary<PlayerHabilityKey, Action>()
            {
                { PlayerHabilityKey.AIM_SHOOT, ChangePlayerShootType},
                { PlayerHabilityKey.WALL_JUMP, WallJumpChangeType  }
            };


            this.SetState(PlayerState.IDLE);
        }

        public void DisableCamera()
        {
            this.CameraPlayer.Enabled = false;
        }

        public void EnableCamera()
        {
            this.CameraPlayer.Enabled = true;
            this.CameraPlayer.MakeCurrent();
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

            Vector2 inputVector = Vector2.Zero;
            CurrentPlayerSpeed = 0f;
            if (this.CurrentPlayerState != PlayerState.WALL_SLIDING
                && !this.PlayeCanMoveCamera)
            {
                if (Input.IsActionPressed("d") && this.IsMovementBlocked == false && this.CurrentPlayerState != PlayerState.AIMING)
                {
                    inputVector.X += 1;
                    FlipRaycast(direction: 1, [AttackRaycast]);
                    CurrentPlayerSpeed = Speed;
                }

                if (Input.IsActionPressed("a") && this.IsMovementBlocked == false && this.CurrentPlayerState != PlayerState.AIMING)
                {
                    inputVector.X -= 1;
                    FlipRaycast(direction: -1, [AttackRaycast]);
                    CurrentPlayerSpeed = Speed;
                }

                // ✅ Sneak deve modificar a velocidade JÁ DEFINIDA, não substituir
                if (Input.IsActionPressed("sneak") && this.CurrentPlayerState != PlayerState.AIMING && inputVector.X != 0)
                {
                    CurrentPlayerSpeed = SneakSpeed;
                    this.HeadCollision.Disabled = true;
                }

                if (Input.IsActionJustReleased("sneak"))
                {
                    this.HeadCollision.Disabled = false;
                }
            }

            if (this.CurrentPlayerState == PlayerState.WALL_SLIDING && !this.PlayeCanMoveCamera)
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

            if (this.PlayeCanMoveCamera)
            {
                if (Input.IsActionPressed("w")) CameraMap.GlobalPosition += new Vector2(0, -5);
                if (Input.IsActionPressed("s")) CameraMap.GlobalPosition += new Vector2(0, 5);
                if (Input.IsActionPressed("d")) CameraMap.GlobalPosition += new Vector2(5, 0);
                if (Input.IsActionPressed("a")) CameraMap.GlobalPosition += new Vector2(-5, 0);
            }

            if (Input.IsActionJustPressed("jump") && this.CurrentPlayerState != PlayerState.AIMING)
            {
                MovementComponent.Jump();
            }

            if (Input.IsActionJustPressed("map"))
            {
                if (PlayeCanMoveCamera)
                {
                    this.CameraPlayer.MakeCurrent();
                    GetTree().Paused = false;
                    this.PlayeCanMoveCamera = false;
                    this.ProcessMode = ProcessModeEnum.Pausable;
                }
                else
                {
                    this.CameraMap.MakeCurrent();
                    GetTree().Paused = true;
                    this.PlayeCanMoveCamera = true;
                    this.ProcessMode = ProcessModeEnum.WhenPaused;
                }
            }

            if (Input.IsActionJustPressed("action"))
            {
                ProcessActionKey();
            }

            if (Input.IsActionJustPressed(nameof(EnumActionsInput.change_type_of_hability)))
            {
                PlayerHabilityKey currentActivePlayerHability = PlayerManager.GetCurrentActivePlayerHability();
                if (currentActivePlayerHability == PlayerHabilityKey.NONE) return;
                if (PlayerHabilityMethodToExecute.TryGetValue(currentActivePlayerHability, out Action action))
                {
                    action();
                }
            }

            MovementComponent.Move(inputVector, CurrentPlayerSpeed);

            base.HandleInput(delta);
        }


        private static void ChangePlayerShootType()
        {
            PlayerManager.GetPlayerGlobalInstance().SetCurrentPlayerShootType(PlayerShootType.AIM_SHOOT);
            GDLogger.LogYellow("Player Shoot Type Changed to: " + PlayerManager.GetPlayerGlobalInstance().GetCurrentPlayerShootType().ToString());
        }

        private static void WallJumpChangeType()
        {
            GDLogger.LogYellow("Wall Jump Activated");
        }

        private void ProcessActionKey()
        {
            if (this.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_OPEN_DOOR && this.GetDoorThatEnemyIs() != null)
            {
                var door = this.GetDoorThatEnemyIs();
                var isLocked = (bool)door.Get("is_locked");
                var requiredKeyName = (string)door.Get("required_key_name");
                door.Call("try_unlock");
            }

            if (this.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_OUT_HIDDEN_PLACE)
            {
                SetCurrentEnabledAction(PlayerCurrentEnabledAction.NONE);

                Vector2 posicaoFinal = new Vector2(this.Position.X - 25.0f, this.Position.Y);
                this.CreateTween().TweenProperty(this, "position", posicaoFinal, 0.2f);

                this.AnimatedSprite2D.Play(EnumAnimations.hide_out.ToString());
                GetTree().CreateTimer(0.2).Timeout += () =>
                {
                    this.CreateTween().TweenProperty(this, "position", this.PlaceOfHiddenPlace, 0.2f);
                    CallDeferred(nameof(OutHiddenPlace));
                };

            }

            if (this.PlayerCurrentEnabledAction == PlayerCurrentEnabledAction.CAN_HIDE)
            {
                this.SetState(PlayerState.HIDDEN);

                Vector2 posicaoFinal = new Vector2(this.Position.X - 15.0f, this.Position.Y);
                this.CreateTween().TweenProperty(this, "position", posicaoFinal, 0.2f);

                this.AnimatedSprite2D.Play(EnumAnimations.hide_in.ToString());
                GetTree().CreateTimer(0.2).Timeout += () =>
                {
                    this.CreateTween().TweenProperty(this, "position", this.PlaceOfHiddenPlace, 0.2f);
                    CallDeferred(nameof(SetHiddenPlace));
                };

            }


        }

        private void SetHiddenPlace()
        {
            UICodigo.EmitSignal("UIChangeVisibility", false);
            this.RemoveMaskOfEnemy();
            this.BlockMovement();
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

        internal void EnterHiddenPlace(Vector2 PlaceToHide)
        {
            this.SetCurrentEnabledAction(PlayerCurrentEnabledAction.CAN_HIDE);
            this.PlaceOfHiddenPlace = PlaceToHide;
        }

        internal PlayerDirection GetDirectionThatPlayerIsLookingAt()
        {
            return AnimatedSprite2D.FlipH ? PlayerDirection.Left : PlayerDirection.Right;
        }
    }
}

public enum PlayerDirection
{
    Left = -1,
    Right = 1
}
enum EnumActionsInput
{
    habilities_key_toggle,
    change_type_of_hability,
    action,
    jump,
    s,
    w,
    a,
    d
}
