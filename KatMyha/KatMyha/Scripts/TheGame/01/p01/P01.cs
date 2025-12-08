using Godot;
using System;

public partial class BaseConfigScenesLevel : Node
{
    public event Action P01OpenFirstDoor;


    public Action OnP01OpenFirstDoor()
    {
        return P01OpenFirstDoor;
    }

}


public partial class P01 : BaseConfigScenesLevel
{
    [Export] private StaticBody2D P01Door;
    public override void _Ready()
    {
        base._Ready();
        this.P01OpenFirstDoor += MP01OpenFirstDoor;
    }

    private void MP01OpenFirstDoor() => P01Door.QueueFree();

}


