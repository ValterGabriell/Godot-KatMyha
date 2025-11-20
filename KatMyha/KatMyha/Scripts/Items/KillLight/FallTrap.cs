using Godot;
using PrototipoMyha;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatMyha.Scripts.Items.KillLight
{
    public partial class FallTrap : RigidBody2D
    {
        [ExportGroup("Fall Trap Settings")]
        [Export] public bool HasToFall { get; private set; }
        [Export] public bool WasShooted { get; set; }
        [Export] public Texture2D TargetAimedTexture2D { get; private set; }
        [Export] public Texture2D ShootedAimedTexture2D { get; private set; }
        [Export] public Texture2D IdleAimedTexture2D { get; private set; }
        public float DistanceToPlayer { get; set; }
        private SignalManager SignalManager => SignalManager.Instance;

        public void _on_area_2d_body_entered(Node2D Node)
        {
            if (Node.IsInGroup(EnumGroups.enemy.ToString()) && !SignalManager.SignalsEmited.GetValueOrDefault(nameof(SignalManager.PlayerHasKillAnEnemy)))
            {
                SignalManager.EmitSignal(nameof(SignalManager.PlayerHasKillAnEnemy));
                SignalManager.Instance.SignalsEmited.Add(nameof(SignalManager.PlayerHasKillAnEnemy), true);
                this.GetNode<Area2D>("Area2D").SetDeferred("monitoring", false);
                GetTree().CreateTimer(1.5f).Timeout += () =>
                {
                    CallDeferred("queue_free");
                };
                Node.CallDeferred("queue_free");
            }
        }

    }
}
