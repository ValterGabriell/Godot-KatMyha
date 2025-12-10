using Godot;
using PrototipoMyha.Player.StateManager;

namespace KatrinaGame.Core
{
    public partial class BasePlayer : CharacterBody2D
    {
        private const float INITAL_MAX_BALLANCE = 500.0f;
        private float Equilibrio = INITAL_MAX_BALLANCE;
        private float BallanceZero = 0.0f;
        private float BallanceMax = INITAL_MAX_BALLANCE;
        [Export] public AudioStreamPlayer2D DeathAudioStreamPlayer2D { get; set; }

        [Signal]
        public delegate void BalanceZeroEventHandler();

        public void AffectBalance(float intensidade)
        {
            if (Equilibrio > BallanceZero)
            {
                Equilibrio -= intensidade;
                GD.Print("Equilibrio: " + Equilibrio);
            }

            if (Equilibrio <= BallanceZero)
            {
                FallDown();
            }

        }

        private void FallDown()
        {
            // Move o jogador levemente para baixo para garantir que saia da colisão
            this.Position += new Vector2(0, 505);

            // Define a velocidade para baixo
            Velocity = new Vector2(Velocity.X, 500);

            EmitSignal(nameof(BalanceZero));
        }

        public void MovePlayerToFallPosition(Vector2 positionToGo)
        {
            this.GlobalPosition = this.GlobalPosition + positionToGo;
        }

        public void DecreaseBallance(float delta)
        {
            if (Equilibrio < BallanceMax)
            {
                Equilibrio += 30f * delta;
                Equilibrio = Mathf.Max(Equilibrio, 0);
            }
        }


        public PlayerState GetCurrentPlayerMovement()
        {
            return this.CurrentPlayerState;
        }

        public void ResetBallance()
        {
            Equilibrio = INITAL_MAX_BALLANCE;
        }

        public void SlowKatrinaVelocity()
        {
            Speed = 50.0f;
        }

        public void NormalizeKatrinaVelocity()
        {
            Speed = 200.0f;
        }

    }
}
