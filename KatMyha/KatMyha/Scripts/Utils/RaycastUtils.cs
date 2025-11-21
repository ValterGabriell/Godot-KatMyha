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


        /// <summary>
        /// Sets the target position of the specified RayCast2D to align with the player's horizontal position,
        /// optionally limiting the vertical range based on the provided range settings.
        /// </summary>
        /// <remarks>If the raycast is configured with a limited range, the vertical component of the
        /// target position will be set to the maximum allowed value rather than the player's actual vertical
        /// position.</remarks>
        /// <param name="PlayerPos">The world position of the player to align the raycast's target with.</param>
        /// <param name="rayCast2D">The RayCast2D instance whose target position will be updated.</param>
        /// <param name="TargetRaycastPosition">A Vector2 representing the target position for the raycast. Its values will be modified to reflect the
        /// player's position and range constraints.</param>
        /// <param name="raycastFollowHasLimitedRange">An object specifying whether the raycast's vertical range should be limited and, if so, the maximum range to
        /// apply.</param>
        public static void LookAtPlayer(Vector2 PlayerPos, RayCast2D rayCast2D, Vector2 TargetRaycastPosition, RaycastFollowHasLimitedRange raycastFollowHasLimitedRange)
        {
            Vector2 localPlayerPos = rayCast2D.ToLocal(PlayerPos);

            // Calcula a distância horizontal entre o raycast e o jogador
            float horizontalDistance = Mathf.Abs(localPlayerPos.X - rayCast2D.TargetPosition.X);

            // Só segue o jogador se estiver dentro do range
            if (!raycastFollowHasLimitedRange.HasLimitedRange || horizontalDistance <= raycastFollowHasLimitedRange.MaxRange)
            {
                TargetRaycastPosition.X = localPlayerPos.X;
            }
            else
            {
                // Mantém a posição horizontal original
                TargetRaycastPosition.X = rayCast2D.TargetPosition.X;
            }

            // Mantém a distância vertical definida no editor
            TargetRaycastPosition.Y = rayCast2D.TargetPosition.Y;
            rayCast2D.TargetPosition = TargetRaycastPosition;
        }

        /// <summary>
        /// Move o RayCast2D horizontalmente como um pêndulo entre dois limites.
        /// </summary>
        /// <param name="rayCast2D">Instância do RayCast2D a ser movimentada.</param>
        /// <param name="amplitude">Distância máxima para cada lado a partir da posição inicial.</param>
        /// <param name="speed">Velocidade da oscilação (quanto maior, mais rápido).</param>
        /// <param name="initialPosition">Posição inicial do RayCast2D (normalmente definida no editor).</param>
        /// <param name="time">Tempo acumulado (use o tempo global ou delta acumulado).</param>
        public static void PendulumRaycast(
            RayCast2D rayCast2D,
            float amplitude,
            float speed,
            Vector2 initialPosition,
            double time
        )
        {
            // Oscilação senoidal: vai de -amplitude até +amplitude
            float offsetX = amplitude * Mathf.Sin((float)(speed * time));
            rayCast2D.TargetPosition = new Vector2(initialPosition.X + offsetX, initialPosition.Y);
        }
    }

    public record RaycastFollowHasLimitedRange(bool HasLimitedRange,float MaxRange);
}
