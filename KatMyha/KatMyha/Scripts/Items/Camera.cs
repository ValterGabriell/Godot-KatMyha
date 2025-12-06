using Godot;
using PrototipoMyha.Utilidades;

public partial class Camera : Node2D
{
    private PlayerManager playerManager;
    private RayCast2D rayCast2D;
    private Vector2 initialRaycastPosition;

    public override void _Ready()
    {
        rayCast2D = GetNode<RayCast2D>("RayCast2D");
        playerManager = PlayerManager.GetPlayerGlobalInstance();
        initialRaycastPosition = rayCast2D.TargetPosition;
    }


}
