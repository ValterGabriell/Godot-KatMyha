using Godot;
using PrototipoMyha.Enemy;
using PrototipoMyha.Enemy.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatMyha.Scripts.Enemies.BaseGuard.Components.Impl.EnemyMovement
{
    public partial class EnemySearchingComponent : Node, IEnemyBaseComponents
    {
        private EnemyBase EnemyBase;

        public EnemySearchingComponent(EnemyBase enemyBase)
        {
            EnemyBase = enemyBase;
        }

        public void Initialize()
        {
            
        }

        public void PhysicsProcess(double delta)
        {
            
        }

        public void Process(double delta)
        {
            
        }
    }
}
