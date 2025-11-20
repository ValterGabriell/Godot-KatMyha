using Godot;
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

    }
}
