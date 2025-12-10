using Godot;
using KatrinaGame.Players;
using PrototipoMyha;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;

public enum PlayerSwitchLightState
{
    CAN_TURN_ON_LIGHT,
    CAN_TURN_OFF_LIGHT,
    CANT_TOGGLE_LIGHT
}

public enum PlayerShootType
{
    DISTRACTION_SHOOT,
    AIM_SHOOT
}

public enum PlayerHabilityKey
{
    WALL_JUMP,
    AIM_SHOOT,
    NONE
}

public enum PlayerSubphaseKey
{
    SUBFASE_1 = 0,
    SUBFASE_2 = 1,
    SUBFASE_3 = 2,
}


public partial class PlayerManager : Node
{
    private Vector2 CurrentPlayerPosition { get; set; }
    private Vector2? LastDistractionBallPosition { get; set; }
    public Vector2 LastPlayerPositionThatMakedSound { get; private set; }

    private static PlayerManager PlayerGlobalInstance = null;

    public MyhaPlayer BasePlayer { get; private set; } = null;
    public static float RunNoiseRadius { get; private set; } = 150f;
    public static float SneakNoiseRadius { get; private set; } = 50f;
    public static float JumpNoiseRadius { get; private set; } = 50f;
    public bool PlayerCanSaveTheGame { get; set; } = false;
    public PlayerSwitchLightState PlayerCanTurnOfTheLight { get; set; } = PlayerSwitchLightState.CANT_TOGGLE_LIGHT;
    private PlayerShootType CurrentPlayerShootType { get; set; } = PlayerShootType.DISTRACTION_SHOOT;

    private Dictionary<PlayerHabilityKey, bool> PlayerHabilities = new Dictionary<PlayerHabilityKey, bool>();
    private Dictionary<PlayerSubphaseKey, bool> PlayerSubphaseKeys = new Dictionary<PlayerSubphaseKey, bool>();
    private PlayerHabilityKey CurrentActivePlayerHability = PlayerHabilityKey.NONE;
    private SignalManager SignalManager;


    public event Action PlayerChangedHability;

    public override void _Ready()
    {
        this.PlayerSubphaseKeys.Add(PlayerSubphaseKey.SUBFASE_1, true);
        var playerInTree = GetTree().GetNodesInGroup("player");
        BasePlayer = playerInTree.Count > 0
            ? playerInTree[0] as MyhaPlayer
            : null;

        CurrentPlayerPosition = playerInTree.Count > 0
            ? BasePlayer.GlobalPosition
            : Vector2.Zero;

        SignalManager = SignalManager.Instance;
        SignalManager.PlayerAcessSubphase += OnPlayerAcessSubphase;
        SignalManager.ElevatorReachFinalPoint += OnElevatorReachFinalPoint;


        if (PlayerGlobalInstance == null)
        {
            PlayerGlobalInstance = this;
        }
        else
        {
            QueueFree();
        }
    }

    private void OnPlayerAcessSubphase(List<Marker2D> _)
    {
        BasePlayer.DisableCamera();
    }

    private void OnElevatorReachFinalPoint(Vector2 vector2)
    {
        this.UpdatePlayerPosition(vector2);
        BasePlayer.GlobalPosition = vector2;
        BasePlayer.EnableCamera();
    }

    public PlayerHabilityKey GetCurrentActivePlayerHability()
    {
        return CurrentActivePlayerHability;
    }

    public void SetCurrentActivePlayerHability(PlayerHabilityKey habilityKey)
    {
        GDLogger.LogRed($"Trying to set current active player hability to {habilityKey}");
        if (IsPlayerHabilityUnlocked(habilityKey))
        {
            GDLogger.LogGreen($"Current active player hability set to {habilityKey}");
            CurrentActivePlayerHability = habilityKey;
            this.PlayerChangedHability?.Invoke();
        }
    }

    public void UnlockPlayerHability(PlayerHabilityKey habilityKey)
    {
        if (!PlayerHabilities.ContainsKey(habilityKey))
        {
            PlayerHabilities.Add(habilityKey, true);
            this.PlayerChangedHability?.Invoke();
        }
    }

    public bool IsPlayerHabilityUnlocked(PlayerHabilityKey habilityKey)
    {
        return PlayerHabilities.ContainsKey(habilityKey) && PlayerHabilities[habilityKey];
    }

    public void UnlockPlayerSubphaseKey(PlayerSubphaseKey subphaseKey)
    {
        if (!PlayerSubphaseKeys.ContainsKey(subphaseKey))
        {
            PlayerSubphaseKeys.Add(subphaseKey, true);
        }
    }

    public List<PlayerSubphaseKey> GetObtainedKeys()
    {
        return new List<PlayerSubphaseKey>(PlayerSubphaseKeys.Keys);
    }

    public List<PlayerHabilityKey> GetUnlockedPlayerHabilities()
    {
        return new List<PlayerHabilityKey>(PlayerHabilities.Keys);
    }

    public bool IsPlayerSubphaseKeyUnlocked(PlayerSubphaseKey subphaseKey)
    {
        return PlayerSubphaseKeys.ContainsKey(subphaseKey) && PlayerSubphaseKeys[subphaseKey];
    }

    public void SetObtainedKeys(List<PlayerSubphaseKey> obtainedKeys)
    {
        foreach (var key in obtainedKeys)
        {
            if (!PlayerSubphaseKeys.ContainsKey(key))
            {
                PlayerSubphaseKeys.Add(key, true);
            }
        }
    }

    public void SetUnlockedPlayerHabilities(List<PlayerHabilityKey> unlockedHabilities)
    {
        foreach (var hability in unlockedHabilities)
        {
            if (!PlayerHabilities.ContainsKey(hability))
            {
                PlayerHabilities.Add(hability, true);
            }
        }
    }

    public void SetCurrentPlayerShootType(PlayerShootType shootType)
    {
        CurrentPlayerShootType = shootType;
    }

    public PlayerShootType GetCurrentPlayerShootType()
    {
        return CurrentPlayerShootType;
    }

    public override void _PhysicsProcess(double delta)
    {
        // Atualiza continuamente a posição do jogador
        if (BasePlayer != null)
        {
            CurrentPlayerPosition = BasePlayer.GlobalPosition;
        }
    }

    public static PlayerManager GetPlayerGlobalInstance()
    {
        return PlayerGlobalInstance;
    }

    public void UpdatePlayerPosition(Vector2 newPosition)
    {
        CurrentPlayerPosition = newPosition;
    }

    public Vector2 GetPlayerPosition()
    {
        return CurrentPlayerPosition;
    }

    public void UpdateLastDistractionBallPosition(Vector2? newPosition)
    {
        LastDistractionBallPosition = newPosition;
    }

    public Vector2 GetLastDistractionBallPosition()
    {
        return LastDistractionBallPosition ?? CurrentPlayerPosition;
    }

    public void UpdateLastPlayerPositionThatMakedSound(Vector2 newPosition)
    {
        LastPlayerPositionThatMakedSound = newPosition;
    }


}
