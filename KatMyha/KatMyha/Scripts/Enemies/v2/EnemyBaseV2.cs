using Godot;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Scripts.Utils.Objetos;
using System;

namespace KatMyha.Scripts.Enemies.DroneEnemy
{
	public partial class EnemyBaseV2 : CharacterBody2D
	{
		[Export] public EnemyResources Resources { get; set; }

		public float CurrentHealth { get; private set; }
		public bool IsDead { get; private set; }

        private Guid Identifier = Guid.NewGuid();

		public EnemyState CurrentEnemyState { get; private set; } = EnemyState.Roaming;

        public override void _Ready()
		{
			if (Resources == null)
			{
				GD.PrintErr("EnemyResources não foi configurado!");
				Resources = new EnemyResources();
			}

			CurrentHealth = Resources.MaxHealth;
			IsDead = false;
		}

		public void SetEnemyState(EnemyState newState)
		{
			CurrentEnemyState = newState;
        }

        public virtual void TakeDamage(float damage)
		{
			if (IsDead) return;

			CurrentHealth -= damage;
			CurrentHealth = Mathf.Max(CurrentHealth, 0);

			if (CurrentHealth <= 0)
			{
				Die();
			}
		}

		protected virtual void Die()
		{
			if (IsDead) return;
			IsDead = true;
		}

        public EnemySaveData ToSaveData()
        {
            return new EnemySaveData
            {
                InstanceID = Identifier,
                PositionX = this.GlobalPosition.X,
                PositionY = this.GlobalPosition.Y,
                EnemyState = this.CurrentEnemyState
            };
        }
    }
}
