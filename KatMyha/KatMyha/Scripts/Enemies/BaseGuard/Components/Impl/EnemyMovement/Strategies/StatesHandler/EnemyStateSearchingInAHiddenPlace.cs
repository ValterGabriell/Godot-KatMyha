using Godot;
using PrototipoMyha;
using PrototipoMyha.Enemy;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Scripts.Enemies.BaseGuard.Components.Impl.EnemyMovement.Strategies.Interfaces;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatMyha.Scripts.Enemies.BaseGuard.Components.Impl.EnemyMovement.Strategies.StatesHandler
{
    internal class EnemyStateSearchingInAHiddenPlace : IEnemyStateHandler
    {
        private bool IsExcute = false;
        private float ElapsedTime = 0f;
        private const float SearchDuration = 2000f; 
    

        public float ExecuteState(
            double delta,
            EnemyBase InEnemy,
            Vector2? InTargetPosition = null)
        {
            if (!this.IsExcute)
            {
                InEnemy.Velocity = Vector2.Zero;
                this.IsExcute = true;
                this.ElapsedTime = 0f;
            }

            if (InEnemy.RayCast2DDetection.IsColliding())
            {
                InEnemy.ProcessKillOfPlayer();
            }
            this.ElapsedTime += (float)delta * 1000f;

            if (this.ElapsedTime >= SearchDuration)
            {
              
                InEnemy.SetState(EnemyState.Waiting);
                this.IsExcute = false;
                this.ElapsedTime = 0f;
            }

            return 2f;
        }
    }
}
