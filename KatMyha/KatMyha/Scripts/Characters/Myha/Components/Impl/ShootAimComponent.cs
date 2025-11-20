using Godot;
using KatMyha.Scripts.Items.KillLight;
using KatrinaGame.Core;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Player.Components.Interfaces;
using PrototipoMyha.Player.StateManager;
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

        private FallTrap currentTargetAimed = null;
        private FallTrap lastAimLightShooted = null;
        private List<FallTrap> allTargetsOnRange = [];
        public void HandleInput(double delta)
        {
            if (Input.IsActionJustPressed("aim")) SignalManager.EmitSignal(nameof(SignalManager.PlayerAim));
            if (Input.IsActionJustReleased("aim")) SignalManager.EmitSignal(nameof(SignalManager.PlayerRemoveAim));
            if (Input.IsActionJustPressed("shoot")) SignalManager.EmitSignal(nameof(SignalManager.PlayerShoot));

        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent)
            {
                if (mouseEvent.ButtonIndex == MouseButton.WheelUp
                    && mouseEvent.Pressed
                    && this.currentTargetAimed != null)
                {
                    SwitchToNextLight();
                }
                else if (mouseEvent.ButtonIndex == MouseButton.WheelDown
                    && mouseEvent.Pressed
                    && this.currentTargetAimed != null)
                {
                    SwitchToNextLight();
                }
            }
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
            if (this._player.CurrentPlayerState == PlayerState.AIMING)
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
            this._player.SetState(PlayerState.IDLE);
        }

        private void OnPlayerAim()
        {
            this._player.SetState(PlayerState.AIMING);
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