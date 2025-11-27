using Godot;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Scripts.Enemies.BaseGuard.Components.Impl.EnemyMovement.Strategies.Interfaces;
using PrototipoMyha.Utilidades;
using System;

namespace PrototipoMyha.Enemy.Components.Impl.EnemyMovement.Strategies.StatesHandler
{
    public class EnemyStateWaitingHandler : IEnemyStateHandler
    {
        private float InWaitTime;
        private Action SetNewRandomTarget;

        public EnemyStateWaitingHandler(float inWaitTime, Action setNewRandomTarget)
        {
            InWaitTime = inWaitTime;
            SetNewRandomTarget = setNewRandomTarget;
        }

        public float ExecuteState(
          double delta,
          EnemyBase InEnemy, Vector2? InPositionToChase = null)
        { 
            InWaitTime -= (float)delta;
            InEnemy.Velocity = Vector2.Zero;
            if (InWaitTime <= 0f)
            {
                if(InEnemy.EnemyResource.EnemyJustMoveWhenDistracted)
                    InEnemy.SetState(EnemyState.StopedWhileNotDistracted);
                else
                    InEnemy.SetState(EnemyState.Roaming);
                SetNewRandomTarget();
                InEnemy.SetPolygonRoamingColor();
            }
            return InWaitTime;
        }


    }
}
