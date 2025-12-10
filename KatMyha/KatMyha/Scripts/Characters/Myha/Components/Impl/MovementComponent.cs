using Godot;
using KatrinaGame.Core;
using KatrinaGame.Core.Interfaces;
using KatrinaGame.Players;
using KatrinaGame.Scripts.Utils;
using PrototipoMyha;
using PrototipoMyha.Player.StateManager;
using System;

namespace KatrinaGame.Components
{
    public partial class MovementComponent : Node, IMovementComponent
    {
        private BasePlayer _player;
        private MyhaPlayer _myhaPlayer;
        private float _inertiaDeceleration = 20f;
        private float _coyoteTime = 0.2f; // Tempo em segundos para o pulo de coyote
        private float _coyoteTimer = 0f;
        private bool _wasOnFloorLastFrame = true; // Adicionar esta variável
        private bool timeToFallStarted = false;
        private int wallDir = 0;

        //usando no walk sound area
        public bool IsPlayerWalking { get; private set; } = false;

        public SignalManager SignalManager { get; private set; } = SignalManager.Instance;

        public void Initialize(BasePlayer player)
        {
            _player = player;
            _myhaPlayer = player as MyhaPlayer;
            this._player.TimeToFallWall.Timeout += FallFromWall;

        }

        private void FallFromWall()
        {
            this._player.SetState(PlayerState.FALLING);
        }

        public void Process(double delta)
        {
        }

        public void PhysicsProcess(double delta)
        {
            ApplyGravity(delta);
            CheckLanding();
            UpdateCoyoteTimer(delta);
        }

        public void HandleInput(double delta)
        {

        }
        private void UpdateCoyoteTimer(double delta)
        {
            if (_player.IsOnFloor() && this._player.CurrentPlayerState != PlayerState.WALL_SLIDING)
            {
                _coyoteTimer = _coyoteTime; // reseta o timer quando está no chão
            }
            else
            {
                _coyoteTimer -= (float)delta; // diminui o timer quando está no ar
            }
        }




        public void Move(Vector2 direction, float CurrentSpeed)
        {
            PlayerManager.GetPlayerGlobalInstance().UpdatePlayerPosition(_player.GlobalPosition);
            if (this._player.IsMovementBlocked) return;



            var isPlayerMoving = this._player.CurrentPlayerState != PlayerState.WALL_SLIDING ? direction.X != 0 : direction.Y != 0;
            var isPlayerOnFloor = this._player.IsOnFloor();
            var isPlayerJumping = this._player.CurrentPlayerState == PlayerState.JUMPING;
            var isPlayerHidden = this._player.CurrentPlayerState == PlayerState.HIDDEN;
            var isPlayerWalLWalking = this._player.CurrentPlayerState == PlayerState.WALL_SLIDING;

            if (isPlayerMoving && this._player.CurrentPlayerState != PlayerState.WALL_SLIDING)
            {
                IsPlayerWalking = true;
                _player.Velocity = new Vector2(direction.Normalized().X * CurrentSpeed, _player.Velocity.Y);

                UpdateSpriteDirection(direction);
            }

            if (isPlayerMoving && this._player.CurrentPlayerState == PlayerState.WALL_SLIDING)
            {
                IsPlayerWalking = true;
                _player.Velocity = new Vector2(0, direction.Normalized().Y * CurrentSpeed);


            }

            if (IsMovingConditionMet(isPlayerMoving, isPlayerOnFloor, isPlayerJumping)
                && !isPlayerJumping
                && this._player.CurrentPlayerState != PlayerState.AIMING
                && this._player.CurrentPlayerState != PlayerState.HIDDEN)
            {
                var newState = CurrentSpeed == _player.Speed ? PlayerState.RUN : PlayerState.SNEAK;
                var newRadiusSound = CurrentSpeed == _player.Speed ? PlayerManager.RunNoiseRadius : PlayerManager.SneakNoiseRadius;
                string newPlayerAnimation = CurrentSpeed == _player.Speed ? EnumAnimations.run.ToString() : EnumAnimations.sneak.ToString();
                this._player.SetState(newState);
                SignalManager.EmitSignal(nameof(SignalManager.PlayerIsMoving), newRadiusSound);
                SignalManager.EmitSignal(nameof(SignalManager.PlayerHasChangedState), newPlayerAnimation);
            }

            if (IsIdleConditionMet(isPlayerMoving, isPlayerOnFloor, isPlayerJumping, isPlayerWalLWalking, isPlayerHidden))
            {
                SignalManager.EmitSignal(nameof(SignalManager.PlayerStoped));
                SignalManager.EmitSignal(nameof(SignalManager.PlayerHasChangedState), EnumAnimations.idle.ToString());
            }



            if (IsWallContactDetected(isPlayerOnFloor) && this._player.Velocity.Y < 0)
            {
                StartWallSlide();
            }

            if (!isPlayerOnFloor && !IsWallSliding() && this._myhaPlayer.CurrentPlayerState == PlayerState.WALL_SLIDING)
                FallFromWall();

            if (!isPlayerMoving)
            {
                IsPlayerWalking = false;

                if (this._player.CurrentPlayerState == PlayerState.WALL_SLIDING)
                {
                    // Reseta tanto X quanto Y na parede
                    _player.Velocity = new Vector2(0, 0);
                }
                else
                {
                    // Reseta apenas X no chão/ar
                    _player.Velocity = new Vector2(
                        Mathf.MoveToward(_player.Velocity.X, 0, _inertiaDeceleration),
                        _player.Velocity.Y
                    );
                }
            }

            CheckLanding();
        }

        private void StartWallSlide()
        {
            if (this._player.CurrentPlayerState == PlayerState.WALL_SLIDING)
            {
                this._player.Velocity
                    = new Vector2(this._player.Velocity.X, Math.Min(this._player.Velocity.Y, this._player.WallWalkSpeed));
            }
        }

        private void StopWallSlide()
        {
            if (this._player.CurrentPlayerState == PlayerState.WALL_SLIDING)
            {
                this._player.SetState(PlayerState.IDLE);
                wallDir = 0;
            }
        }

        private bool IsWallContactDetected(bool isPlayerOnFloor)
        {
            return !isPlayerOnFloor && IsWallSliding();
        }

        private bool IsWallSliding()
        {
            return this._player.CurrentPlayerState == PlayerState.WALL_SLIDING;
        }

        private bool IsPlayerRising()
        {
            return _player.Velocity.Y < 0;
        }

        public void Jump()
        {
            bool canJump = _player.IsOnFloor()
                || this._player.CurrentPlayerState == PlayerState.WALL_SLIDING || _coyoteTimer > 0f;

            if (this._player.IsMovementBlocked || !canJump) return;

            if (this._player.CurrentPlayerState == PlayerState.WALL_SLIDING)
            {
                ApplyWallJump();
                return;
            }
            ApplyJump();
        }


        private void ApplyJump()
        {
            this._player.SetState(PlayerState.JUMPING);
            _player.Velocity = new Vector2(_player.Velocity.X, _player.JumpVelocity);

            SignalManager.EmitSignal(nameof(SignalManager.PlayerIsMoving), PlayerManager.JumpNoiseRadius);
            SignalManager.EmitSignal(nameof(SignalManager.PlayerHasChangedState), EnumAnimations.jump_up.ToString());
        }

        private void ApplyWallJump()
        {
            this._player.SetState(PlayerState.JUMPING_WALL);

            _player.Velocity = new Vector2(0, _player.JumpVelocity);

            SignalManager.EmitSignal(nameof(SignalManager.PlayerIsMoving), PlayerManager.JumpNoiseRadius);
            SignalManager.EmitSignal(nameof(SignalManager.PlayerHasChangedState), EnumAnimations.jump_up.ToString());
        }

        private static bool IsIdleConditionMet(bool isPlayerMoving, bool isPlayerOnFloor, bool isPlayerJumping, bool isPlayerWalLWalking, bool isPlayerHidden)
        {
            return !isPlayerMoving && !isPlayerJumping && isPlayerOnFloor && !isPlayerWalLWalking && !isPlayerHidden;
        }

        private static bool IsMovingConditionMet(bool isPlayerMoving, bool isPlayerOnFloor, bool isPlayerJumping)
        {
            return isPlayerMoving && isPlayerOnFloor && !isPlayerJumping;
        }

        private void UpdateSpriteDirection(Vector2 direction)
        {
            if (direction.X > 0)
            {
                _player.AnimatedSprite2D.FlipH = false;
            }
            else if (direction.X < 0)
            {
                _player.AnimatedSprite2D.FlipH = true;
            }
        }


        // Adicionar este método para detectar quando o jogador pousa
        private void CheckLanding()
        {
            /*
            Frame 1: Jogador no chão → _wasOnFloorLastFrame = true
            Frame 2: Jogador pula → _player.IsOnFloor() = false → _wasOnFloorLastFrame = false  
            Frame 3: Jogador no ar → _wasOnFloorLastFrame = false
            Frame 4: Jogador pousa → _player.IsOnFloor() = true, _wasOnFloorLastFrame = false
                     ↳ DETECÇÃO DE POUSO! (!false && true = true)
            Frame 5: Após detecção → _wasOnFloorLastFrame = true
             */
            bool isOnFloorNow = _player.IsOnFloor();

            // Se não estava no chão no frame anterior mas agora está (acabou de pousar)
            if (!_wasOnFloorLastFrame && isOnFloorNow && _player.CurrentPlayerState == PlayerState.JUMPING)
            {
                // CORREÇÃO: Ao pousar, verifica se está se movendo para escolher o estado apropriado
                if (IsPlayerWalking)
                {
                    _player.SetState(PlayerState.RUN);
                }
                else
                {
                    _player.SetState(PlayerState.IDLE);
                }
            }

            _wasOnFloorLastFrame = isOnFloorNow;
        }

        public void ApplyGravity(double delta)
        {

            if (!_player.IsOnFloor())
            {
                _player.Velocity = new Vector2(_player.Velocity.X, _player.Velocity.Y + _player.Gravity * (float)delta);
            }
        }

        public bool IsWallWalkInactive()
        {
            return this._player.CurrentPlayerState != PlayerState.WALL_SLIDING;
        }

        public bool IsWallWalkActive()
        {
            return this._player.CurrentPlayerState == PlayerState.WALL_SLIDING;
        }
        public void Cleanup() { }
    }
}
