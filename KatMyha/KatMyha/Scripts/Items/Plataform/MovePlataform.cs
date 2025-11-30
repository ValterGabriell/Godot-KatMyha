using Godot;
using System;

public partial class MovePlataform : Path2D
{
    [Export] public bool IsLooping = true;
    [Export] public float Speed = 2.0f;
    private float SpeedScale = 1.0f;
    [Export] public PathFollow2D PathFollow2D;
    [Export] public AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        if (IsLooping)
        {
            PathFollow2D.Loop = true;
            AnimationPlayer.Play("move_plataform");
            AnimationPlayer.SpeedScale = SpeedScale;
        }
    }

    public override void _Process(double delta)
    {
        this.PathFollow2D.Progress += Speed;
    }
}
