using Godot;
using PrototipoMyha.Player.StateManager;
using System;
using System.Collections.Generic;

namespace PrototipoMyha;

public partial class SignalManager : Node
{
    [Signal] public delegate void EnemyEnteredWarningAreaEventHandler(Vector2 InPositionToGo);
    [Signal] public delegate void EnemyRatStopLookForPlayerEventHandler(Vector2 InPositionToGo);
    [Signal] public delegate void PlayerStateChangedEventHandler(PlayerState NewState);
    [Signal] public delegate void EnemySpottedPlayerEventHandler();
    [Signal] public delegate void EnemySpottedPlayerShowAlertEventHandler(Vector2 positionToShowAlert);
    [Signal] public delegate void PlayerIsMovingEventHandler(float NoiseValue);
    [Signal] public delegate void PlayerHasChangedStateEventHandler(string animationToPlay);
    [Signal] public delegate void PlayerStopedEventHandler();
    [Signal] public delegate void EnemyKillMyhaEventHandler();
    [Signal] public delegate void PlayerSaveTheGameEventHandler();
    [Signal] public delegate void PlayerAimEventHandler();
    [Signal] public delegate void PlayerRemoveAimEventHandler();
    [Signal] public delegate void PlayerShootEventHandler();
    [Signal] public delegate void PlayerIsOnLightEventHandler();
    [Signal] public delegate void PlayerHasAlterStateOfLightEventHandler(string playerSwitchLightState);
    [Signal] public delegate void PlayerHasKillAnEnemyEventHandler();

    /*actions*/
    public Action<List<Marker2D>> PlayerAcessSubphase;
    public Action<Vector2> ElevatorReachFinalPoint;


    [Signal] public delegate void GameLoadedEventHandler(Vector2 position);


    public Dictionary<string, bool> SignalsEmited = new();

    private static SignalManager _instance;

    public static SignalManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GD.PushError("SignalManager instance accessed before initialization!");
            }
            return _instance;
        }
    }


    public override void _EnterTree()
    {
        if (_instance != null && _instance != this)
        {
            GD.PushWarning("Multiple SignalManager instances detected. Destroying duplicate.");
            QueueFree();
            return;
        }

        _instance = this;
    }

    public override void _ExitTree()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}

/*
 SINAL: EnemyKillMyha | EMISSOR: EnemyStateChaseAlertedBase | PARÂMETROS: Nenhum | Observador: GameManager, EnemyAnimationComponent
 SINAL: PlayerIsMoving | EMISSOR: MovementComponent | PARÂMETROS: Nenhum | Observador: MakeSoundWhileWalking
 
 
 */
