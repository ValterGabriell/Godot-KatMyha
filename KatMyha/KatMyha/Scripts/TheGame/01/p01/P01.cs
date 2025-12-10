using Godot;



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


