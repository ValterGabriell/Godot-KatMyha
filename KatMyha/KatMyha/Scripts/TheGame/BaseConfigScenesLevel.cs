using System;

public partial class BaseConfigScenesLevel : Godot.Node
{
    public event Action P01OpenFirstDoor;

    public Action OnP01OpenFirstDoor()
    {
        return P01OpenFirstDoor;
    }
}
