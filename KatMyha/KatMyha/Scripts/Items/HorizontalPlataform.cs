using Godot;
using PrototipoMyha;
using PrototipoMyha.Utilidades;
using System;

public partial class HorizontalPlataform : StaticBody2D
{
    [Export] private bool HasToFall { get; set; }
    [Export] private float TimeToFall { get; set; } = 2f;
    public void _on_area_2d_body_entered(Node2D node2D)
    {
        if (node2D.IsInGroup(EnumGroups.player.ToString()) && HasToFall)
        {
            GDLogger.LogGreen("Player on plataform, starting fall timer");
            GetTree().CreateTimer(TimeToFall).Timeout += () =>
            {
                GDLogger.LogYellow("Plataform falling");
                QueueFree();
            };
        }

    }
}
