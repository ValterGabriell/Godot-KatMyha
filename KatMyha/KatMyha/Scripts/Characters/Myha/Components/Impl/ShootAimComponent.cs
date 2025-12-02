using Godot;
using KatMyha.Scripts.Items.KillLight;
using KatrinaGame.Core;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Player.Components.Interfaces;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Utilidades;
using System.Collections.Generic;
using System.Linq;

namespace KatMyha.Scripts.Characters.Myha.Components.Impl
{
    public interface IShootAimComponent : IPlayerBaseComponent
    {
    }
    public partial class ShootAimComponent : Node, IShootAimComponent
    {
        private MyhaPlayer _player;
        public SignalManager SignalManager { get; private set; } = SignalManager.Instance;
        private PlayerManager PlayerManager { get; } = PlayerManager.GetPlayerGlobalInstance();

        private FallTrap currentTargetAimed = null;
        private FallTrap lastAimLightShooted = null;
        private List<FallTrap> allTargetsOnRange = [];

        // Componentes para mira de distração
        private Line2D trajectoryLine;
        private Sprite2D landingIndicator;
        private const float TRAJECTORY_SIMULATION_STEPS = 30f;
        private const float DEFAULT_THROW_FORCE = 200f;

        // Força do arremesso agora é variável e ajustável
        private float currentThrowForce = DEFAULT_THROW_FORCE;
        private const float MIN_THROW_FORCE = 300f;
        private const float MAX_THROW_FORCE = 900f;
        private const float THROW_FORCE_STEP = 50f; // Incremento por scroll

        private const float DISTRACTION_GRAVITY = 980f;
        private const float THROW_VERTICAL_MULTIPLIER = 0.5f;
        private readonly float MAX_THROW_DISTANCE = 400f;
        private bool isDistractionAimActive = false;

        public override void _Ready()
        {
            base._Ready();
            CreateDistractionReticle();
        }

        public void HandleInput(double delta)
        {
            if (Input.IsActionJustPressed("aim")) SignalManager.EmitSignal(nameof(SignalManager.PlayerAim));
            if (Input.IsActionJustReleased("aim")) SignalManager.EmitSignal(nameof(SignalManager.PlayerRemoveAim));
            if (Input.IsActionJustPressed("shoot")) SignalManager.EmitSignal(nameof(SignalManager.PlayerShoot));

            // Atualizar trajetória quando o mouse se move durante aim de distração
            if (isDistractionAimActive)
            {
                UpdateDistractionTrajectory();
            }
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent)
            {
                // Ajustar força do arremesso com roda do mouse no modo distração
                if (isDistractionAimActive)
                {
                    if (mouseEvent.ButtonIndex == MouseButton.WheelUp && mouseEvent.Pressed)
                    {
                        AdjustThrowForce(THROW_FORCE_STEP);
                        return;
                    }
                    else if (mouseEvent.ButtonIndex == MouseButton.WheelDown && mouseEvent.Pressed)
                    {
                        AdjustThrowForce(-THROW_FORCE_STEP);
                        return;
                    }
                }

                // Trocar luz alvo no modo aim shoot
                if (this.currentTargetAimed != null)
                {
                    if (mouseEvent.ButtonIndex == MouseButton.WheelUp && mouseEvent.Pressed)
                    {
                        SwitchToNextLight();
                    }
                    else if (mouseEvent.ButtonIndex == MouseButton.WheelDown && mouseEvent.Pressed)
                    {
                        SwitchToNextLight();
                    }
                }
            }

            // Atualizar trajetória quando o mouse se move
            if (@event is InputEventMouseMotion && isDistractionAimActive)
            {
                UpdateDistractionTrajectory();
            }
        }

        private void AdjustThrowForce(float delta)
        {
            currentThrowForce = Mathf.Clamp(currentThrowForce + delta, MIN_THROW_FORCE, MAX_THROW_FORCE);
            UpdateDistractionTrajectory(); // Atualizar trajetória imediatamente

            // Feedback visual opcional (você pode adicionar um label para mostrar a força atual)
            GDLogger.LogBlue($"Throw Force: {currentThrowForce:F0}");
        }

        private void ChangeAimLightToAim()
        {
            this.currentTargetAimed = this.allTargetsOnRange[
                   (this.allTargetsOnRange.IndexOf(this.currentTargetAimed) + 1) % this.allTargetsOnRange.Count
               ];
            if (this.currentTargetAimed != null && !this.currentTargetAimed.WasShooted)
            {
                this.currentTargetAimed.GetNode<Sprite2D>("Sprite2D").Texture
                    = currentTargetAimed.TargetAimedTexture2D;
            }
        }

        private void CreateDistractionReticle()
        {
            // Criar linha de trajetória
            trajectoryLine = new Line2D
            {
                Width = 2f,
                DefaultColor = new Color(1, 1, 0, 0.6f), // Amarelo semi-transparente
                Visible = false
            };
            AddChild(trajectoryLine);

            // Criar indicador de pouso
            landingIndicator = new Sprite2D
            {
                Modulate = new Color(1, 0, 0, 0.7f), // Vermelho semi-transparente
                Scale = new Vector2(0.5f, 0.5f),
                Visible = false
            };

            // Criar uma textura circular simples para o indicador
            var indicatorTexture = new GradientTexture2D();
            var gradient = new Gradient();
            gradient.SetColor(0, new Color(1, 0, 0, 0.8f));
            gradient.SetColor(1, new Color(1, 0, 0, 0));
            indicatorTexture.Gradient = gradient;
            indicatorTexture.Fill = GradientTexture2D.FillEnum.Radial;
            indicatorTexture.Width = 64;
            indicatorTexture.Height = 64;

            landingIndicator.Texture = indicatorTexture;
            AddChild(landingIndicator);
        }

        private void SwitchToNextLight()
        {
            if (this.allTargetsOnRange.Count > 0)
            {
                ChangeAimLightToAim();
                ResetOtherLightsTexture();
            }
        }

        private void ResetOtherLightsTexture()
        {
            this.allTargetsOnRange.Where(e => e.Name != this.currentTargetAimed.Name && !e.WasShooted).Select(e =>
            {
                e.GetNode<Sprite2D>("Sprite2D").Texture = e.IdleAimedTexture2D;
                return e;
            }).ToList();
        }

        private void BackToPreviusLight()
        {
            if (this.allTargetsOnRange.Count > 0)
            {
                this.currentTargetAimed = this.allTargetsOnRange[
                    (this.allTargetsOnRange.IndexOf(this.currentTargetAimed) + 1) % this.allTargetsOnRange.Count
                ];
                this.allTargetsOnRange.Where(e => e.Name != this.currentTargetAimed.Name && !e.WasShooted).Select(e =>
                {
                    e.GetNode<Sprite2D>("Sprite2D").Texture = e.IdleAimedTexture2D;
                    return e;
                }).ToList();
                ChangeAimLightToAim();
            }
        }

        public void Initialize(BasePlayer player)
        {
            _player = player as MyhaPlayer;
            SignalManager.Instance.PlayerAim += OnPlayerAim;
            SignalManager.Instance.PlayerRemoveAim += OnPlayerRemoveAim;
            SignalManager.Instance.PlayerShoot += OnPlayerShoot;
        }

        private void OnPlayerShoot()
        {
            if (this._player.CurrentPlayerState == PlayerState.AIMING && PlayerManager.GetPlayerGlobalInstance().GetCurrentPlayerShootType() == PlayerShootType.AIM_SHOOT)
            {
                if (this.currentTargetAimed == null || this.currentTargetAimed.WasShooted) return;

                this._player.SetState(PlayerState.SHOOTING);

                this.currentTargetAimed.WasShooted = true;
                this.currentTargetAimed.GetNode<Sprite2D>("Sprite2D").Texture = currentTargetAimed.ShootedAimedTexture2D;

                this.lastAimLightShooted = this.currentTargetAimed;
                this.allTargetsOnRange.Remove(this.currentTargetAimed);

                if (this.currentTargetAimed.HasToFall)
                {
                    this.currentTargetAimed.GravityScale = 1;
                    var shootedTrap = this.currentTargetAimed;
                    var timer = GetTree().CreateTimer(3.0);
                    timer.Timeout += () => shootedTrap?.QueueFree();
                }

                if (this.allTargetsOnRange.Count > 0)
                {
                    this._player.SetState(PlayerState.AIMING);
                    BackToPreviusLight();
                }
                else
                {
                    this.currentTargetAimed = null;
                    this._player.SetState(PlayerState.IDLE);
                }
            }

            if(this._player.CurrentPlayerState == PlayerState.AIMING && PlayerManager.GetPlayerGlobalInstance().GetCurrentPlayerShootType() == PlayerShootType.DISTRACTION_SHOOT)
            {
                this._player.SetState(PlayerState.THROW);
                ThrowDistractionBall();
                HideDistractionReticle();
                isDistractionAimActive = false;
                this._player.SetState(PlayerState.IDLE);
            }
        }

        private void ThrowDistractionBall()
        {
            if (_player?.BallScene == null) return;

            var ball = _player.BallScene.Instantiate<RigidBody2D>();
            GetTree().Root.AddChild(ball);
            ball.GlobalPosition = _player.GlobalPosition;

            var direction = GetFacingDirection();

            // Adicionar componente vertical (Y negativo = para cima no Godot)
            var horizontalVelocity = direction.X * currentThrowForce;
            var verticalVelocity = -currentThrowForce * THROW_VERTICAL_MULTIPLIER; // Ajuste o multiplicador para controlar a altura do arco

            ball.LinearVelocity = new Vector2(horizontalVelocity, verticalVelocity);
            ball.GravityScale = DISTRACTION_GRAVITY / 980f;
        }

        private void OnPlayerRemoveAim()
        {
            foreach (var trap in this.allTargetsOnRange)
            {
                if (!trap.WasShooted)
                {
                    trap.GetNode<Sprite2D>("Sprite2D").Texture = trap.IdleAimedTexture2D;
                }
            }
            this.currentTargetAimed = null;

            HideDistractionReticle();
            isDistractionAimActive = false;
            currentThrowForce = DEFAULT_THROW_FORCE; 

            this._player.SetState(PlayerState.IDLE);
        }

        private void OnPlayerAim()
        {
            if(!PlayerManager.IsPlayerHabilityUnlocked(PlayerHabilityKey.AIM_SHOOT)) return;
            this._player.SetState(PlayerState.AIMING);
            if (PlayerManager.GetPlayerGlobalInstance().GetCurrentPlayerShootType() == PlayerShootType.AIM_SHOOT)
            {
                FindAndSetAimTargets();
                HideDistractionReticle();
                isDistractionAimActive = false;
                return;
            }

            // Modo de distração
            ShowDistractionReticle();
            isDistractionAimActive = true;
            currentThrowForce = DEFAULT_THROW_FORCE; 
            UpdateDistractionTrajectory();
 
        }

        private void UpdateDistractionTrajectory()
        {
            if (_player == null) return;

            var startPos = _player.GlobalPosition;
            var direction = GetFacingDirection();

            trajectoryLine.ClearPoints();

            var horizontalVelocity = direction.X * currentThrowForce;
            var verticalVelocity = -currentThrowForce * THROW_VERTICAL_MULTIPLIER; 
            var velocity = new Vector2(horizontalVelocity, verticalVelocity);

            var landingPos = SimulateTrajectory(startPos, velocity);

            landingIndicator.GlobalPosition = landingPos;
        }

        private Vector2 GetFacingDirection()
        {
            float facingDirection = _player.AnimatedSprite2D.FlipH ? -1f : 1f;
            return new Vector2(facingDirection, 0);
        }

        private Vector2 SimulateTrajectory(Vector2 startPos, Vector2 velocity)
        {
            Vector2 currentPos = startPos;
            Vector2 landingPos = startPos;

            for (int i = 0; i < TRAJECTORY_SIMULATION_STEPS; i++)
            {
                float t = i / 60f;
                Vector2 nextPos = CalculateParabolicPosition(startPos, velocity, t);

                if (IsOverMaxDistance(startPos, nextPos, currentPos, out Vector2 clampedPos))
                {
                    trajectoryLine.AddPoint(clampedPos - _player.GlobalPosition);
                    return clampedPos;
                }

                trajectoryLine.AddPoint(nextPos - _player.GlobalPosition);

                if (CheckGroundCollision(currentPos, nextPos, out Vector2 collisionPos))
                {
                    return collisionPos;
                }

                currentPos = nextPos;
                landingPos = nextPos;
            }

            return landingPos;
        }

        private Vector2 CalculateParabolicPosition(Vector2 startPos, Vector2 velocity, float time)
        {
            return startPos + velocity * time + new Vector2(0, 0.5f * DISTRACTION_GRAVITY * time * time);
        }

        private bool IsOverMaxDistance(Vector2 startPos, Vector2 nextPos, Vector2 currentPos, out Vector2 clampedPosition)
        {
            float totalDistance = startPos.DistanceTo(nextPos);

            if (totalDistance > MAX_THROW_DISTANCE)
            {
                float excessRatio = (totalDistance - MAX_THROW_DISTANCE) / currentPos.DistanceTo(nextPos);
                clampedPosition = currentPos.Lerp(nextPos, 1f - excessRatio);
                return true;
            }

            clampedPosition = Vector2.Zero;
            return false;
        }

        private bool CheckGroundCollision(Vector2 from, Vector2 to, out Vector2 collisionPosition)
        {
            var spaceState = _player.GetWorld2D().DirectSpaceState;
            var query = PhysicsRayQueryParameters2D.Create(from, to);
            query.CollideWithAreas = false;
            query.CollideWithBodies = true;
            query.CollisionMask = (1 << 0) | (1 << 1);

            var result = spaceState.IntersectRay(query);

            if (result.Count > 0)
            {
                collisionPosition = (Vector2)result["position"];
                return true;
            }

            collisionPosition = Vector2.Zero;
            return false;
        }

        private void ShowDistractionReticle()
        {
            trajectoryLine.Visible = true;
            landingIndicator.Visible = true;
        }

        private void HideDistractionReticle()
        {
            trajectoryLine.Visible = false;
            landingIndicator.Visible = false;
        }

        private void FindAndSetAimTargets()
        {
            var allLights = GetTree().GetNodesInGroup(EnumGroups.fall_trap.ToString());
            var playerPos = _player.GlobalPosition;
            float nearestDist = 300f;

            this.allTargetsOnRange.Clear();

            foreach (var node in allLights)
            {
                if (node is not FallTrap fallTrap)
                    continue;

                float dist = fallTrap.GlobalPosition.DistanceTo(playerPos);
                if (dist < nearestDist && !fallTrap.WasShooted)
                {
                    fallTrap.DistanceToPlayer = dist;
                    this.allTargetsOnRange.Add(fallTrap);
                    fallTrap.GetNode<Sprite2D>("Sprite2D").Texture = fallTrap.IdleAimedTexture2D;
                }
            }

            if (this.allTargetsOnRange.Count > 0)
            {
                this.currentTargetAimed = this.allTargetsOnRange.MinBy(e => e.DistanceToPlayer);
                ChangeAimLightToAim();
            }
        }

        public void PhysicsProcess(double delta)
        {

        }

        public void Process(double delta)
        {

        }
    }
}
