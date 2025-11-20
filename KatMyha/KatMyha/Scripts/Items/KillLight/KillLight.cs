using Godot;
using KatMyha.Scripts.Items.KillLight;
using PrototipoMyha;

public partial class KillLight : FallTrap
{
    [Export] private float TextureScale = 1.0f;
    [Export] private float EnergyOfLight = 1.0f;
    [Export]  private PointLight2D PointLight2D { get; set; }
    [Export] private Sprite2D LightSprite { get; set; }
    [Export] private Area2D Area2D { get; set; }

    private SignalManager SignalManager => SignalManager.Instance;

    private float LightFadeFactor = 0.1f;
    public override void _Ready()
    {
        SignalManager.PlayerHasAlterStateOfLight += OnPlayerHasAlterStateOfLight;
    }

    private void OnPlayerHasAlterStateOfLight(string playerSwitchLightState)
    {
        bool hasToggleOff = playerSwitchLightState == PlayerSwitchLightState.CAN_TURN_OFF_LIGHT.ToString();
        if (hasToggleOff)
        {
            PointLight2D.Energy = 0;
            Area2D.Monitoring = false;
            Area2D.GetNode<CollisionPolygon2D>("CollisionPolygon2D").Disabled = true;
        }

        if (!hasToggleOff && PointLight2D.Energy == 0)
        {
            PointLight2D.Energy = 1;
            Area2D.Monitoring = true;
            Area2D.GetNode<CollisionPolygon2D>("CollisionPolygon2D").Disabled = false;
        }
    }
}
