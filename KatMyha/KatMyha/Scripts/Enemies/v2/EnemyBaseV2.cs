using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy.States;
using KatMyha.Scripts.Utils;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Scripts.Utils.Objetos;
using System;

namespace KatMyha.Scripts.Enemies.DroneEnemy
{
	public partial class EnemyBaseV2 : CharacterBody2D, IDistanceToSelf
    {
		[Export] public EnemyResources Resources { get; set; }

		public float CurrentHealth { get; private set; }
		public bool IsDead { get; private set; }

        private Guid Identifier = Guid.NewGuid();
        public Guid GetIdentifier()
        {
            return Identifier;
        }

        public void SetIdentifier(Guid guid)
        {
            this.Identifier = guid;
        }
        public bool JustLoaded { get; set; } = false;
        public EnumEnemyState CurrentEnemyState { get; private set; } = EnumEnemyState.Idle;

		public EnemyStateBase EnemyStateBase => GetNode<StateMachine>("StateMachine").GetCurrentState();

        /// <summary>
        /// This is setted by FindItemsNearest when searching for nearest items.
        /// </summary>
        float IDistanceToSelf.DistanceToSelf { get; set; }

        public float DistanceToSelf = 0f;

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

		public void SetEnemyState(EnumEnemyState newState)
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
