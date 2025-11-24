using Godot;
using KatMyha.Scripts.Managers;
using KatrinaGame.Core;
using KatrinaGame.Players;
using KatrinaGame.Scripts.Utils;
using PrototipoMyha.Enemy;
using PrototipoMyha.Player.Components.Interfaces;
using PrototipoMyha.Player.StateManager;

namespace PrototipoMyha.Player.Components.Impl
{
    public interface IMakeSoundWhileWalkComponent : IPlayerBaseComponent
    {
    }
    public partial class MakeSoundWhileWalkComponent : Node, IMakeSoundWhileWalkComponent
    {
        private MyhaPlayer MyhaPlayer;
        private SignalManager SignalManager = SignalManager.Instance;
        private SoundManager SoundManager = SoundManager.Instance;

        public MakeSoundWhileWalkComponent(MyhaPlayer BasePlayer)
        {
            this.MyhaPlayer = BasePlayer;
        }

        public void HandleInput(double delta) { }

        public void Initialize(BasePlayer player)
        {
            this.MyhaPlayer.SoundAreaWalkingComponent.BodyEntered += OnBodyEntered;
            this.SignalManager.PlayerIsMoving += OnMyhaIsMoving;
            this.SignalManager.PlayerHasChangedState += OnPlayerHasStateChanged;
            this.SignalManager.PlayerSaveTheGame += OnPlayerSaveGame;
            this.SignalManager.PlayerStoped += OnMyhaStoped;
        }

        private void OnPlayerSaveGame()
        {
            SoundManager.Instance.PlaySound(this.MyhaPlayer.SaveAudioStreamPlayer2D, soundExtension: SoundExtension.wav);
        }

        private void OnPlayerHasStateChanged(string animationToPlay)
        {
            if (animationToPlay == EnumAnimations.jump_up.ToString())
            {
                SoundManager.Instance.PlaySound(this.MyhaPlayer.JumpAudioStreamPlayer2D, soundExtension: SoundExtension.wav);
            }

        }

        private void OnMyhaStoped()
        {
            if (this.MyhaPlayer.CurrentPlayerState != PlayerState.AIMING && this.MyhaPlayer.CurrentPlayerState != PlayerState.HIDDEN)
            {
                this.MyhaPlayer.SetState(PlayerState.IDLE);
                this.MyhaPlayer.AlterRadiusCollisionSoundArea(0);
            }

        }

        private void OnMyhaIsMoving(float NoiseValue)
        {
            if (MyhaPlayer.CurrentPlayerState == PlayerState.SNEAK && MyhaPlayer.CurrentLightHiddenState == LightHiddenState.LIGHT_HIDDEN)
            {
                this.MyhaPlayer.AlterRadiusCollisionSoundArea(0);
                return;
            }
            this.MyhaPlayer.AlterRadiusCollisionSoundArea(NoiseValue);
            SoundManager.Instance.PlaySound(this.MyhaPlayer.WalkAudioStreamPlayer2D, soundExtension: SoundExtension.wav);
        }

        private void OnBodyEntered(Node2D area)
        {
            if (area is EnemyBase enemy && enemy.CurrentEnemyState != Enemy.States.EnemyState.Chasing)
            {
                enemy.SetState(Enemy.States.EnemyState.Alerted);
            }
        }



        public void Process(double delta) { }

        public void PhysicsProcess(double delta) { }
    }
}
