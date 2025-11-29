using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy;
using KatMyha.Scripts.Enemies.DroneEnemy.States;
using KatMyha.Scripts.Managers;
using KatrinaGame.Core;
using PrototipoMyha;
using PrototipoMyha.Scripts.Utils;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatMyha.Scripts.Enemies.BaseGuardV2.States
{
    public partial class BaseGuardStateChasing : EnemyStateBase
    {
        private BaseGuardV2 BaseGuardV2 { get; set; }
        private bool hasEmittedKillSignal = false;
        private SignalManager SignalManager;
        private SoundManager SoundManager = SoundManager.Instance;
        private bool stopEnemy = false;
        public BaseGuardStateChasing(EnemyBaseV2 enemy, StateMachine stateMachine) : base(enemy, stateMachine)
        {
            this.BaseGuardV2 = enemy as BaseGuardV2;
            SignalManager = SignalManager.Instance;
        }

        public override void PhysicsProcess(float delta)
        {
            ProcessKillOfPlayer(BaseGuardV2);
        }

        public override void Process(float delta)
        {
           
        }

        private void ProcessKillOfPlayer(BaseGuardV2 InEnemy)
        {
            if (InEnemy.RayCast2DDetection != null)
            {
                (BasePlayer player, bool isColliding) = RaycastUtils.IsColliding<BasePlayer>(InEnemy.RayCast2DDetection);

                if (isColliding
                    && !hasEmittedKillSignal)
                {
                    SignalManager.EmitSignal(nameof(SignalManager.EnemyKillMyha));
                    SoundManager.PlaySound(player.DeathAudioStreamPlayer2D);
                    InEnemy.Velocity = Vector2.Zero;
                    hasEmittedKillSignal = true;
                    InEnemy.Velocity = Vector2.Zero;
                }
            }
        }
    }
}
