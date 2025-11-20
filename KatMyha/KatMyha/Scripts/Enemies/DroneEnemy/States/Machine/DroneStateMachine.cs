using Godot;
using PrototipoMyha.Enemy.States;

namespace KatMyha.Scripts.Enemies.DroneEnemy.States
{
 
    public partial class DroneStateMachine : StateMachine
    {
        protected override void InitializeStates(EnemyBaseV2 _enemy)
        {
            RegisterState(EnemyState.Roaming, new EnemyRoamingState(_enemy));
            RegisterState(EnemyState.Chasing, new EnemyChasingState(_enemy));
            RegisterState(EnemyState.Alerted, new EnemyAlertedState(_enemy));
        }
    }
}
