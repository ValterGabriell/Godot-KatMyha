using Godot;

namespace KatMyha.Scripts.Enemies.DroneEnemy
{
    [GlobalClass]
    public partial class EnemyResources : Resource
    {
        [ExportGroup("Health")]
        [Export] public float MaxHealth { get; set; } = 100f;
        [Export] public float HealthRegeneration { get; set; } = 0f;

        [ExportGroup("Movement")]
        [Export] public float MoveSpeed { get; set; } = 150f;
        [Export] public float Acceleration { get; set; } = 500f;
        [Export] public float Deceleration { get; set; } = 500f;

        [ExportGroup("Combat")]
        [Export] public float AttackDamage { get; set; } = 10f;
        [Export] public float AttackSpeed { get; set; } = 1f;
        [Export] public float AttackRange { get; set; } = 200f;
        [Export] public float AttackCooldown { get; set; } = 1.5f;

        [ExportGroup("Detection")]
        [Export] public float DetectionRadius { get; set; } = 300f;
        [Export] public float LoseTargetDistance { get; set; } = 400f;
        [Export] public float VisionAngle { get; set; } = 360f;

        [ExportGroup("Behavior")]
        [Export] public float RoamRadius { get; set; } = 200f;
        [Export] public float IdleTime { get; set; } = 2f;
        [Export] public float ChaseTime { get; set; } = 5f;

        [ExportGroup("Rewards")]
        [Export] public int ExperiencePoints { get; set; } = 50;
        [Export] public int GoldDrop { get; set; } = 10;
        [Export] public float DropChance { get; set; } = 0.3f;

        public EnemyResources Duplicate()
        {
            return new EnemyResources
            {
                MaxHealth = MaxHealth,
                HealthRegeneration = HealthRegeneration,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration,
                Deceleration = Deceleration,
                AttackDamage = AttackDamage,
                AttackSpeed = AttackSpeed,
                AttackRange = AttackRange,
                AttackCooldown = AttackCooldown,
                DetectionRadius = DetectionRadius,
                LoseTargetDistance = LoseTargetDistance,
                VisionAngle = VisionAngle,
                RoamRadius = RoamRadius,
                IdleTime = IdleTime,
                ChaseTime = ChaseTime,
                ExperiencePoints = ExperiencePoints,
                GoldDrop = GoldDrop,
                DropChance = DropChance
            };
        }
    }
}