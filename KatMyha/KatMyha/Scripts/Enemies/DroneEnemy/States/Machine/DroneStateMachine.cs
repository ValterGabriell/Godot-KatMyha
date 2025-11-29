using Godot;
using PrototipoMyha.Enemy.States;

namespace KatMyha.Scripts.Enemies.DroneEnemy.States
{
 
    public partial class DroneStateMachine : StateMachine
    {
        protected override void InitializeStates(EnemyBaseV2 _enemy)
        {
            RegisterState(EnumEnemyState.Roaming, new EnemyRoamingState(_enemy, this));
            RegisterState(EnumEnemyState.Chasing, new EnemyKillingState(_enemy, this));
        }
    }
}
