using Godot;
using KatMyha.Scripts.Managers;
using KatrinaGame.Core;
using PrototipoMyha.Scripts.Enemies.BaseGuard.Components.Impl.EnemyMovement.Strategies.Interfaces;
using PrototipoMyha.Scripts.Utils;
using PrototipoMyha.Utilidades;
using static Godot.TextServer;

namespace PrototipoMyha.Enemy.Components.Impl.EnemyMovement.Strategies.StatesHandler.Chase_Alerted
{
    public class EnemyStateChaseAlertedBase : IEnemyStateHandler
    {
        private Vector2 InTargetMovement;
        private bool IsInvestigatingArea = false;
        /// <summary>
        /// foi criado porque havia um bug que o raycast ficava doido quando o inimigo chegava proximo do local, mudando
        /// rapidamente de 1 pra -1
        /// </summary>
        private float RaycastDirectionWhenStartToAlert = 0f;

        // Threshold para considerar se estão no mesmo nível vertical
        private const float VerticalLevelThreshold = 30f;
        private const int TIME_TO_LOOK_UP_DOWN = 200;
        private const int TIME_TO_WAIT_WHEN_WAITING_START = 3;
        private int controlTimeToLookUpDown = 0;
        private SignalManager SignalManager;
        private bool hasEmittedKillSignal = false;
        private bool stopEnemy = false;
        private SoundManager SoundManager = SoundManager.Instance;
        
        // NOVO: Controle de timeout para evitar que o inimigo fique preso
        private float timeStuckAtPosition = 0f;
        private Vector2 lastPosition = Vector2.Zero;
        private const float MAX_STUCK_TIME = 5.0f; // 5 segundos sem progresso = desistir
        private const float STUCK_DISTANCE_THRESHOLD = 2f; // Se mover menos que 2 pixels por segundo = preso
        private const float STUCK_CHECK_INTERVAL = 1.0f; // Verificar a cada 1 segundo
        private float timeSinceLastCheck = 0f;

        public EnemyStateChaseAlertedBase(Vector2 inTargetMovement)
        {
            InTargetMovement = inTargetMovement;
            SignalManager = SignalManager.Instance;
            lastPosition = Vector2.Zero; // Será inicializado no primeiro frame
        }

        public virtual float ExecuteState(
           double delta,
           EnemyBase InEnemy,
           Vector2? InPositionToChase = null)
        {
            //chase
            if (InPositionToChase.HasValue)
                this.InTargetMovement = ClampToBoundaries(InEnemy, InPositionToChase.Value);
            else 
            {
                // Para DistractionAlerted, não clampar a posição para permitir que o inimigo investigue fora dos limites
                if (InEnemy.CurrentEnemyState == States.EnemyState.DistractionAlerted)
                {
                    this.InTargetMovement = InTargetMovement;
                }
                else
                {
                    this.InTargetMovement = ClampToBoundaries(InEnemy, InTargetMovement);
                }
            }

            return HandleAlertedMovement(InEnemy, delta);
        }

        private Vector2 ClampToBoundaries(EnemyBase enemy, Vector2 position)
        {
            if (enemy.Marker_01 == null || enemy.Marker_02 == null)
                return position;

            float minX = Mathf.Min(enemy.Marker_01.GlobalPosition.X, enemy.Marker_02.GlobalPosition.X);
            float maxX = Mathf.Max(enemy.Marker_01.GlobalPosition.X, enemy.Marker_02.GlobalPosition.X);

            position.X = Mathf.Clamp(position.X, minX, maxX);

            return position;
        }

        private float HandleAlertedMovement(EnemyBase InEnemy, double delta)
        {
            Vector2 directionToPlayer = (InTargetMovement - InEnemy.GlobalPosition).Normalized();

            // Verifica se o jogador não está no mesmo nível vertical
            bool isPlayerAtDifferentLevel = IsPlayerAtDifferentVerticalLevel(InEnemy);

            // Verifica se chegou no boundary
            bool isAtBoundary = IsAtBoundaryLimit(InEnemy);
            
            // Calcula a distância horizontal para o alvo
            float horizontalDistanceToTarget = Mathf.Abs(InTargetMovement.X - InEnemy.GlobalPosition.X);
            
            // NOVO: Detectar se o inimigo está preso (não se movendo)
            if (InEnemy.CurrentEnemyState == States.EnemyState.DistractionAlerted)
            {
                CheckIfStuck(InEnemy, delta);
                
                // Se ficou preso por muito tempo, desistir e voltar para Waiting
                if (timeStuckAtPosition >= MAX_STUCK_TIME)
                {
                    GDLogger.LogRed($"Enemy stuck for {MAX_STUCK_TIME}s, giving up on distraction");
                    InEnemy.SetState(States.EnemyState.Waiting);
                    InEnemy.Velocity = Vector2.Zero;
                    ResetStuckTimer();
                    return TIME_TO_WAIT_WHEN_WAITING_START;
                }
            }

            // Se o jogador estiver em nível diferente, pare o movimento horizontal
            float horizontalVelocity = 0f;
            
            // Para Alerted normal, respeitar boundary
            // Para DistractionAlerted, ignorar boundary completamente
            if (!isPlayerAtDifferentLevel)
            {
                if (InEnemy.CurrentEnemyState == States.EnemyState.DistractionAlerted)
                {
                    // DistractionAlerted: SEMPRE se mover, ignorando boundary
                    horizontalVelocity = directionToPlayer.X * InEnemy.EnemyResource.ChaseSpeed;
                }
                else
                {
                    // Alerted normal: respeitar boundary
                    if (!isAtBoundary)
                    {
                        horizontalVelocity = directionToPlayer.X * InEnemy.EnemyResource.ChaseSpeed;
                    }
                }
            }

            InEnemy.Velocity = new Vector2(horizontalVelocity, InEnemy.Velocity.Y);


            ProcessAlertAtDifferentLevel(InEnemy, directionToPlayer, isPlayerAtDifferentLevel, horizontalVelocity);
            HandleAlertedStateTransition(InEnemy, isPlayerAtDifferentLevel, directionToPlayer, isAtBoundary);
            
            if (IsRaycastDirectionNotInitialized() && !isAtBoundary)
            {
                FlipEnemyDirection(InEnemy, directionToPlayer);
            }

            ProcessKillOfPlayer(InEnemy);
            if (stopEnemy)
            {
                InEnemy.Velocity = Vector2.Zero;
            }

            return TIME_TO_WAIT_WHEN_WAITING_START;
        }

        private void ProcessKillOfPlayer(EnemyBase InEnemy)
        {
            if (InEnemy.RayCast2DDetection != null)
            {
                (BasePlayer player, bool isColliding) = RaycastUtils.IsColliding<BasePlayer>(InEnemy.RayCast2DDetection);

                if (isColliding
                    && !hasEmittedKillSignal)
                {
                    SignalManager.EmitSignal(nameof(SignalManager.EnemyKillMyha));
                    SoundManager.PlaySound(player.DeathAudioStreamPlayer2D);
                    InEnemy.Velocity = Vector2.Zero;
                    hasEmittedKillSignal = true;
                    stopEnemy = true;
                }
            }
        }

        // NOVO MÉTODO: Verifica se o inimigo está no limite dos marcadores
        private bool IsAtBoundaryLimit(EnemyBase enemy)
        {
            if (enemy.Marker_01 == null || enemy.Marker_02 == null)
                return false;

            float minX = Mathf.Min(enemy.Marker_01.GlobalPosition.X, enemy.Marker_02.GlobalPosition.X);
            float maxX = Mathf.Max(enemy.Marker_01.GlobalPosition.X, enemy.Marker_02.GlobalPosition.X);

            const float BOUNDARY_THRESHOLD = 10f; // Margem de erro

            // Verifica se está próximo de qualquer limite
            bool isAtMinBoundary = Mathf.Abs(enemy.GlobalPosition.X - minX) < BOUNDARY_THRESHOLD;
            bool isAtMaxBoundary = Mathf.Abs(enemy.GlobalPosition.X - maxX) < BOUNDARY_THRESHOLD;

            return isAtMinBoundary || isAtMaxBoundary;
        }

        private float ProcessAlertAtDifferentLevel(EnemyBase InEnemy, Vector2 directionToPlayer, bool isPlayerAtDifferentLevel, float horizontalVelocity)
        {
            // Ativa animação específica quando o jogador está em nível diferente
            if (isPlayerAtDifferentLevel && InEnemy.CurrentEnemyState != States.EnemyState.Chasing)
            {
                //move o inimigo na direção do jogador
                horizontalVelocity = directionToPlayer.X * InEnemy.EnemyResource.ChaseSpeed;
                InEnemy.Velocity = new Vector2(horizontalVelocity, InEnemy.Velocity.Y);

                //caso esteja perto o suficiente do jogador no eixo X, para olhar para cima ou para baixo
                var diff = Mathf.Abs(InEnemy.Position.X - InTargetMovement.X);
                if (diff < 3)
                {
                    InEnemy.Velocity = Vector2.Zero;
                    ActivateLookUpDownAnimation(InEnemy);
                }

            }

            return horizontalVelocity;
        }


        private void HandleAlertedStateTransition(EnemyBase InEnemy, bool isPlayerAtDifferentLevel, Vector2 directionToPlayer, bool isAtBoundary)
        {
            float direction = directionToPlayer.X > 0 ? 1 : -1;
            // Calcula apenas a distância horizontal (eixo X)
            float horizontalDistanceToTarget = Mathf.Abs(InTargetMovement.X - InEnemy.GlobalPosition.X);
            
            float threshold = 15f;
            
            if ((InEnemy.CurrentEnemyState == States.EnemyState.Alerted || InEnemy.CurrentEnemyState == States.EnemyState.DistractionAlerted)
                && horizontalDistanceToTarget < threshold && !IsInvestigatingArea && !isPlayerAtDifferentLevel)
            {
                ToggleRaycastDirectionOnAlert(direction);
                InEnemy.SetState(States.EnemyState.Waiting);
                
                // Resetar stuck timer quando transicionar com sucesso
                if (InEnemy.CurrentEnemyState == States.EnemyState.DistractionAlerted)
                {
                    ResetStuckTimer();
                }
            }
        }

        private static void FlipEnemyDirection(EnemyBase InEnemy, Vector2 direction)
        {
            int directionSign = direction.X > 0 ? 1 : -1;

            RaycastUtils.FlipRaycast(directionSign, [InEnemy.RayCast2DDetection]);
            SpriteUtils.FlipSprite(directionSign, InEnemy.AnimatedSprite2DEnemy);
            PolyngUtils.Flip(directionSign, InEnemy.Polygon2DDetection);
        }


        /// <summary>
        /// Verifica se o jogador está em um nível vertical diferente do inimigo
        /// </summary>
        /// <param name="enemy">O inimigo para comparar posição</param>
        /// <returns>True se o jogador estiver em nível diferente, False caso contrário</returns>
        private bool IsPlayerAtDifferentVerticalLevel(EnemyBase enemy)
        {
            float verticalDistance = Mathf.Abs(InTargetMovement.Y - enemy.GlobalPosition.Y);
            return verticalDistance > VerticalLevelThreshold;
        }

        /// <summary>
        /// Ativa a animação específica quando o jogador está em nível vertical diferente
        /// </summary>
        /// <param name="enemy">O inimigo que deve executar a animação</param>
        private void ActivateLookUpDownAnimation(EnemyBase enemy)
        {
            controlTimeToLookUpDown++;
            // Determina se o jogador está acima ou abaixo
            bool isPlayerAbove = InTargetMovement.Y < enemy.GlobalPosition.Y;
      
            // Exemplo de como ativar animação específica
            if (isPlayerAbove)
            {
                // Animação olhando para cima
                GDLogger.Log("Player is above - Enemy stopped and looking up");
            }
            else
            {
                // Animação olhando para baixo
                GDLogger.LogGreen("Player is above - Enemy stopped and looking up");

            }

            if (controlTimeToLookUpDown > TIME_TO_LOOK_UP_DOWN)
            {
                enemy.SetState(States.EnemyState.Waiting);
                controlTimeToLookUpDown = 0;
            }
              
        }

        private bool IsRaycastDirectionNotInitialized()
        {
            return RaycastDirectionWhenStartToAlert == 0;
        }

        private void ToggleRaycastDirectionOnAlert(float raycastDirection)
        {
            if (RaycastDirectionWhenStartToAlert == 0)
                RaycastDirectionWhenStartToAlert = raycastDirection;
            else
                RaycastDirectionWhenStartToAlert = 0f;
        }

        private void CheckIfStuck(EnemyBase enemy, double delta)
        {
            timeSinceLastCheck += (float)delta;
            
            // Só verificar a cada intervalo para evitar falsos positivos
            if (timeSinceLastCheck < STUCK_CHECK_INTERVAL)
            {
                return;
            }
            
            // Inicializar lastPosition na primeira verificação
            if (lastPosition == Vector2.Zero)
            {
                lastPosition = enemy.GlobalPosition;
                timeStuckAtPosition = 0f;
                timeSinceLastCheck = 0f;
                return;
            }
            
            float distanceMoved = enemy.GlobalPosition.DistanceTo(lastPosition);
            
            // Se moveu muito pouco no intervalo, está preso
            if (distanceMoved < STUCK_DISTANCE_THRESHOLD)
            {
                timeStuckAtPosition += timeSinceLastCheck;
                GDLogger.LogYellow($"Enemy possibly stuck. Moved {distanceMoved:F2}px in {timeSinceLastCheck:F2}s. Total stuck time: {timeStuckAtPosition:F2}s");
            }
            else
            {
                // Resetar o timer se conseguiu se mover
                if (timeStuckAtPosition > 0)
                {
                    GDLogger.LogGreen($"Enemy unstuck! Moved {distanceMoved:F2}px");
                }
                timeStuckAtPosition = 0f;
            }
            
            lastPosition = enemy.GlobalPosition;
            timeSinceLastCheck = 0f;
        }

        private void ResetStuckTimer()
        {
            timeStuckAtPosition = 0f;
            lastPosition = Vector2.Zero;
            timeSinceLastCheck = 0f;
        }
    }
}