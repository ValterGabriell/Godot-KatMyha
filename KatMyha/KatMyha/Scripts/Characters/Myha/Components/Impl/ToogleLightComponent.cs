using Godot;
using KatrinaGame.Core;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Player.Components.Interfaces;

namespace KatMyha.Scripts.Characters.Myha.Components.Impl
{
    public interface IToogleLightComponent : IPlayerBaseComponent
    {
    }
    internal partial class ToogleLightComponent : Node, IToogleLightComponent
    {
        private MyhaPlayer MyhaPlayer;
        private PlayerManager PlayerManager => PlayerManager.GetPlayerGlobalInstance();
        private SignalManager SignalManager => SignalManager.Instance;
        public void HandleInput(double delta)
        {
            if (CanToggleLight())
            {
                PlayerManager.PlayerCanTurnOfTheLight = PlayerManager.PlayerCanTurnOfTheLight == PlayerSwitchLightState.CAN_TURN_ON_LIGHT
                  ? PlayerSwitchLightState.CAN_TURN_OFF_LIGHT : PlayerSwitchLightState.CAN_TURN_ON_LIGHT;
                SignalManager
                    .EmitSignal(nameof(SignalManager.Instance.PlayerHasAlterStateOfLight), PlayerManager.PlayerCanTurnOfTheLight.ToString()) ;
            }
        }

        private bool CanToggleLight()
        {
            return Input.IsActionJustPressed("f")
                            && PlayerManager.PlayerCanTurnOfTheLight != PlayerSwitchLightState.CANT_TOGGLE_LIGHT;
        }

        public void Initialize(BasePlayer player)
        {
            MyhaPlayer = player as MyhaPlayer;
        }

        public void PhysicsProcess(double delta)
        {

        }

        public void Process(double delta)
        {

        }
    }
}
