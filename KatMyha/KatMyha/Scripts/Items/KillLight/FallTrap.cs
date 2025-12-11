using Godot;
using PrototipoMyha;
using PrototipoMyha.Scripts.Utils.Objetos;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;

namespace KatMyha.Scripts.Items.KillLight
{
    public partial class FallTrap : RigidBody2D
    {
        [ExportGroup("Fall Trap Settings")]
        [Export] public bool HasToFall { get; private set; }
        [Export] public bool WasShooted { get; set; }
        [Export] public Texture2D TargetAimedTexture2D { get; private set; }
        [Export] public Texture2D ShootedAimedTexture2D { get; private set; }
        [Export] public Texture2D IdleAimedTexture2D { get; private set; }

        /// <summary>
        /// Gets or sets the primary roaming marker used for navigation or positioning.
        ///  <summary>
        [Export] protected Marker2D RoamMarkerA { get; set; } = null;

        /// <summary>
        /// Gets or sets the secondary roaming marker used for navigation or positioning.
        /// </summary>
        [Export] protected Marker2D RoamMarkerB { get; set; } = null;

        public float DistanceToPlayer { get; set; }
        private SignalManager SignalManager => SignalManager.Instance;
        public Guid InstanceID { get; set; } = Guid.NewGuid();

        public void _on_area_2d_body_entered(Node2D Node)
        {
            if (Node.IsInGroup(EnumGroups.enemy.ToString()) && !SignalManager.SignalsEmited.GetValueOrDefault(nameof(SignalManager.PlayerHasKillAnEnemy)))
            {
                SignalManager.EmitSignal(nameof(SignalManager.PlayerHasKillAnEnemy));
                SignalManager.Instance.SignalsEmited.Add(nameof(SignalManager.PlayerHasKillAnEnemy), true);
                this.GetNode<Area2D>("Area2D").SetDeferred("monitoring", false);
                GetTree().CreateTimer(1.5f).Timeout += () =>
                {
                    CallDeferred("queue_free");
                };
                Node.CallDeferred("queue_free");
            }
        }

        internal MarkerData GetMarkerData_A()
        {
            if (RoamMarkerA is null || RoamMarkerB is null) return null;
            GDLogger.LogRed($"[SAVE] FallTrap Pos: {GlobalPosition}");
            GDLogger.LogRed($"[SAVE] MarkerA GlobalPos: {RoamMarkerA.GlobalPosition}");
            GDLogger.LogRed($"[SAVE] MarkerA LocalPos: {RoamMarkerA.Position}");
            return new MarkerData
            {
                PositionX = RoamMarkerA.Position.X,  // SALVAR LOCAL!
                PositionY = RoamMarkerA.Position.Y   // SALVAR LOCAL!
            };

        }

        public void SetMarkerData_A(MarkerData markerData)
        {

            if (markerData is null) return;
            if (RoamMarkerA is null || RoamMarkerB is null) return;
            GDLogger.LogBlue($"[FallTrap]  A SetMarkerData_A: X={markerData.PositionX}, Y={markerData.PositionY}");
            RoamMarkerA.GlobalPosition = new Vector2(markerData.PositionX, markerData.PositionY);
        }

        public void SetMarkerData_B(MarkerData markerData)
        {
            if (markerData is null) return;
            if (RoamMarkerA is null || RoamMarkerB is null) return;
            GDLogger.LogBlue($"[FallTrap]  A SetMarkerData_B: X={markerData.PositionX}, Y={markerData.PositionY}");
            RoamMarkerB.GlobalPosition = new Vector2(markerData.PositionX, markerData.PositionY);

        }

        public void SetMarkerPositions(float markerAX, float markerAY, float markerBX, float markerBY)
        {
            // Primeiro, deletar marcadores existentes para evitar conflitos
            if (RoamMarkerA != null)
            {
                RoamMarkerA.QueueFree();
                RoamMarkerA = null;
            }
            
            if (RoamMarkerB != null)
            {
                RoamMarkerB.QueueFree();
                RoamMarkerB = null;
            }

            // Criar novos marcadores
            RoamMarkerA = new Marker2D { Name = "RoamMarkerA" };
            RoamMarkerB = new Marker2D { Name = "RoamMarkerB" };
            
            AddChild(RoamMarkerA);
            AddChild(RoamMarkerB);
            
            RoamMarkerA.Owner = this;
            RoamMarkerB.Owner = this;
            
            GD.Print($"[LOAD] Objeto GlobalPosition: {GlobalPosition}");
            GD.Print($"[LOAD] Marcadores criados - MarkerA Local={RoamMarkerA.Position}, MarkerB Local={RoamMarkerB.Position}");
            GD.Print($"[LOAD] Recebendo posições LOCAIS - MarkerA Local=({markerAX}, {markerAY}), MarkerB Local=({markerBX}, {markerBY})");
            
            // Agora as posições JÁ SÃO LOCAIS! Apenas definir diretamente
            RoamMarkerA.Position = new Vector2(markerAX, markerAY);
            RoamMarkerB.Position = new Vector2(markerBX, markerBY);
            
            GD.Print($"[LOAD] Marcadores DEPOIS - MarkerA Global={RoamMarkerA.GlobalPosition}, Local={RoamMarkerA.Position}");
            GD.Print($"[LOAD] Marcadores DEPOIS - MarkerB Global={RoamMarkerB.GlobalPosition}, Local={RoamMarkerB.Position}");
            
            // Reinicializar imediatamente
            ReinitializeMarkers();
        }
        
        private void VerifyAndReinitializeMarkers()
        {
            GD.Print($"[LOAD] Marcadores DEPOIS - MarkerA Global={RoamMarkerA.GlobalPosition}, Local={RoamMarkerA.Position}");
            GD.Print($"[LOAD] Marcadores DEPOIS - MarkerB Global={RoamMarkerB.GlobalPosition}, Local={RoamMarkerB.Position}");
            ReinitializeMarkers();
        }

        internal MarkerData GetMarkerData_B()
        {
            if (RoamMarkerA is null || RoamMarkerB is null) return null;
            GDLogger.LogRed($"[SAVE] MarkerB GlobalPos: {RoamMarkerB.GlobalPosition}");
            GDLogger.LogRed($"[SAVE] MarkerB LocalPos: {RoamMarkerB.Position}");
            return new MarkerData
            {
                PositionX = RoamMarkerB.Position.X,  // SALVAR LOCAL!
                PositionY = RoamMarkerB.Position.Y   // SALVAR LOCAL!
            };
        }

        /// <summary>
        /// Reinitializes marker targets after loading. Must be called after setting marker data.
        /// </summary>
        public virtual void ReinitializeMarkers()
        {
            // Override in derived classes if needed
        }

        public virtual bool GetIsTargetingMarkerA()
        {
            return false;
        }

        public virtual void SetTargetMarker(bool isTargetingMarkerA)
        {
        }

        public virtual void PrepareForLoad()
        {
        }
    }
}
