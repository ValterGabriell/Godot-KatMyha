using Godot;
using KatrinaGame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototipoMyha.Utilidades
{
    public static class RaycastUtils
    {
        public static void FlipRaycast(float direction, List<RayCast2D> Rasycast)
        {
            foreach (var current in Rasycast)
            {
                current.TargetPosition 
                    = new Vector2(direction * Mathf.Abs(current.TargetPosition.X), current.TargetPosition.Y);
            }
        }

        public static (T, bool) IsColliding<T>(RayCast2D rayCast2D)
        {
            if(rayCast2D.IsColliding() && rayCast2D.GetCollider() is T t)
            {
                return (t, true);
            }
            return (default, false);
        }
        public static void LookAtPlayer(Vector2 PlayerPos, RayCast2D rayCast2D, Vector2 TargetRaycastPosition, RaycastFollowHasLimitedRange raycastFollowHasLimitedRange)
        {
            Vector2 localPlayerPos = rayCast2D.ToLocal(PlayerPos);
            TargetRaycastPosition.X = localPlayerPos.X;
            TargetRaycastPosition.Y = raycastFollowHasLimitedRange.HasLimitedRange ? raycastFollowHasLimitedRange.MaxRange : localPlayerPos.Y;
            rayCast2D.TargetPosition = TargetRaycastPosition;
        }
    }

    public record RaycastFollowHasLimitedRange(bool HasLimitedRange,float MaxRange);
}
