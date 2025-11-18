using Godot;
using KatMyha.Scripts.Enemies.BaseGuard.Components.Impl.EnemyMovement.Strategies.StatesHandler;
using PrototipoMyha.Enemy.Components.Impl.EnemyMovement.Strategies.StatesHandler.Chase_Alerted;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Scripts.Enemies.BaseGuard.Components.Impl.EnemyMovement.Strategies.Interfaces;
using PrototipoMyha.Utilidades;
using System;

namespace PrototipoMyha.Enemy.Components.Impl.EnemyMovement.Strategies.StatesHandler
{
    public class GetEnemyStateHandler
    {   
        public static IEnemyStateHandler GetStateHandler(
            EnemyState state,
            float WaitTime,
            float MaxWaitTime,
            Action SetNewWaitTimeWhenWaiting
            )
        {
            
            return state switch
            {
                EnemyState.Roaming => new EnemyStateRoamingHandler(WaitTime, MaxWaitTime),
                EnemyState.Waiting => new EnemyStateWaitingHandler(WaitTime, SetNewWaitTimeWhenWaiting),
                //vector 0 pq o target position vai ser setado em tempo de execucao, referente ao player
                EnemyState.Chasing => new EnemyStateChasingHandler(Vector2.Zero),
                EnemyState.Alerted => new EnemyStateAlertedHandler(PlayerManager.GetPlayerGlobalInstance().GetPlayerPosition()),
                EnemyState.SearchingHiddenPlace => new EnemyStateSearchingInAHiddenPlace(),
                EnemyState.Investigating => null,
                _ => throw new NotImplementedException($"State handler for {state} is not implemented."),
            };
        }
    }
}
