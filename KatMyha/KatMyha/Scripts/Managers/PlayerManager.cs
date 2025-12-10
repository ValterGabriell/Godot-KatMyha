using Godot;
using KatrinaGame.Core;
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

public partial class PlayerManager : Node
{
    private Vector2 CurrentPlayerPosition { get; set; }
    private Vector2? LastDistractionBallPosition { get; set; }
    public Vector2 LastPlayerPositionThatMakedSound { get; private set; }

    private static PlayerManager PlayerGlobalInstance = null;

    public BasePlayer BasePlayer { get; private set; } = null;
    public static float RunNoiseRadius { get; private set; } = 150f;
    public static float SneakNoiseRadius { get; private set; } = 50f;
    public static float JumpNoiseRadius { get; private set; } = 50f;
    public bool PlayerCanSaveTheGame { get; set; } = false;
    public PlayerSwitchLightState PlayerCanTurnOfTheLight { get; set; } = PlayerSwitchLightState.CANT_TOGGLE_LIGHT;
    private PlayerShootType CurrentPlayerShootType { get; set; } = PlayerShootType.DISTRACTION_SHOOT;

    private Dictionary<PlayerHabilityKey, bool> PlayerHabilities = new Dictionary<PlayerHabilityKey, bool>();
    private PlayerHabilityKey CurrentActivePlayerHability = PlayerHabilityKey.NONE;

    public event Action PlayerChangedHability;

    public override void _Ready()
    {
        ////PlayerHabilities.Add(PlayerHabilityKey.WALL_JUMP, true);
        //PlayerHabilities.Add(PlayerHabilityKey.AIM_SHOOT, true);


        var playerInTree = GetTree().GetNodesInGroup("player");
        BasePlayer = playerInTree.Count > 0
            ? playerInTree[0] as BasePlayer
            : null;

        CurrentPlayerPosition = playerInTree.Count > 0
            ? BasePlayer.GlobalPosition
            : Vector2.Zero;



        if (PlayerGlobalInstance == null)
        {
            PlayerGlobalInstance = this;
        }
        else
        {
            QueueFree();
        }
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
