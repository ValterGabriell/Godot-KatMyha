using KatMyha.Scripts.Enemies.DroneEnemy;
using KatMyha.Scripts.Enemies.DroneEnemy.States;
using PrototipoMyha.Enemy.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KatMyha.Scripts.Enemies.BaseGuardV2.States.Machine
{
    public partial class BaseGuardMachine : StateMachine
    {
        protected override void InitializeStates(EnemyBaseV2 _enemy)
        {
            RegisterState(EnumEnemyState.Roaming, new BaseGuardStateRoaming(_enemy, this));
            RegisterState(EnumEnemyState.Waiting, new BaseGuardStateWaiting(_enemy, this));
            RegisterState(EnumEnemyState.Alerted, new BaseGuardStateAlertedBySound(_enemy, this));
            RegisterState(EnumEnemyState.Chasing, new BaseGuardStateChasing(_enemy, this));
        }
    }
}
