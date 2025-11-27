using Godot;
using PrototipoMyha.Enemy;
using PrototipoMyha.Scripts.Enemies.BaseGuard.Components.Impl.EnemyMovement.Strategies.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatMyha.Scripts.Enemies.BaseGuard.Components.Impl.EnemyMovement.Strategies.StatesHandler
{
    internal class EnemyStateStoppedWhileNotDistracted : IEnemyStateHandler
    {
        #warning devo passar a posicao inicial do inimigo para ele voltar pra la!
        public float ExecuteState(double delta, EnemyBase InEnemy, Vector2? InTargetPosition = null)
        {
            InEnemy.Velocity = Vector2.Zero;
            return 0f;
        }
    }
}
