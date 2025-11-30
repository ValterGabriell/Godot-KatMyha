using Godot;
using KatrinaGame.Core;
using KatrinaGame.Core.Interfaces;
using KatrinaGame.Players;
using KatrinaGame.Scripts.Utils;
using PrototipoMyha.Player.Components.Interfaces;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrototipoMyha.Scripts.Characters.Myha.Components.Impl
{
    public interface IAnimationComponents : IPlayerBaseComponent
    {
    }
    public partial class AnimationComponents : Node, IAnimationComponents
    {
        private BasePlayer _player;
        private MyhaPlayer MyhaPlayer;
        SignalManager SignalManager;

        public AnimationComponents(BasePlayer player)
        {
            _player = player;
            MyhaPlayer = player as MyhaPlayer;
        }

        public void HandleInput(double delta)
        {
            
        }

        public void Initialize(BasePlayer player)
        {
          this._player = player;
            this.SignalManager = SignalManager.Instance;
            this.SignalManager.PlayerHasChangedState += OnPlayerHasChangedState;
        }

        private void OnPlayerHasChangedState(string animationToPlay)
        {
            this._player.AnimatedSprite2D.Play(animationToPlay);
            if (
                 (MyhaPlayer.CurrentPlayerState == PlayerState.SNEAK
                 && MyhaPlayer.CurrentLightHiddenState == MyhaContactLightHiddenState.MYHA_IS_NOT_ON_LIGHT)
                 || (MyhaPlayer.CurrentPlayerState == PlayerState.RUN
                 && MyhaPlayer.CurrentLightHiddenState == MyhaContactLightHiddenState.MYHA_IS_NOT_ON_LIGHT)
                 || (MyhaPlayer.CurrentPlayerState == PlayerState.JUMPING
                 && MyhaPlayer.CurrentLightHiddenState == MyhaContactLightHiddenState.MYHA_IS_NOT_ON_LIGHT)
                 || (MyhaPlayer.CurrentPlayerState == PlayerState.JUMPING_WALL
                 && MyhaPlayer.CurrentLightHiddenState == MyhaContactLightHiddenState.MYHA_IS_NOT_ON_LIGHT)
             )
            {
                this._player.SoundAnimatedSprite2D.Visible = false;
                this._player.SoundAnimatedSprite2D.Stop();
                return;
            }
            this._player.SoundAnimatedSprite2D.Visible = true;
            this._player.SoundAnimatedSprite2D.Play(animationToPlay);

        }

        public void PhysicsProcess(double delta)
        {
            
        }

        public void Process(double delta)
        {

        }
    }
}
