using Godot;
using PrototipoMyha.Utilidades;

public partial class Camera : Node2D
{
    private PlayerManager playerManager;
    private RayCast2D rayCast2D;
    private Vector2 initialRaycastPosition;
    private double tempoAcumulado = 0;

    public override void _Ready()
    {
        rayCast2D = GetNode<RayCast2D>("RayCast2D");
        playerManager = PlayerManager.GetPlayerGlobalInstance();
        initialRaycastPosition = rayCast2D.TargetPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        tempoAcumulado += delta;
        if (playerManager != null)
        {
            RaycastUtils.PendulumRaycast(rayCast2D, 100f, 1.5f, initialRaycastPosition, tempoAcumulado);
        }
       
    }
}
