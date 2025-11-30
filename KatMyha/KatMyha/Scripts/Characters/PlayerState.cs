namespace PrototipoMyha.Player.StateManager;

public enum PlayerState
{
    IDLE,
    WALK,
    RUN,
    ATTACK,

    THROW,
    HURT,
    DEAD,
    SNEAK,
    HIDDEN,
    APPEAR,
    JUMPING,
    JUMPING_WALL,
    WALL_SLIDING,
    FALLING,
    AIMING,
    SHOOTING
}


/// <summary>
/// Specifies the actions that a player can currently perform.
/// </summary>
/// <remarks>This enumeration defines the set of actions that are enabled for a player in the game context. Each
/// value represents a specific action that the player is allowed to execute.</remarks>
public enum PlayerCurrentEnabledAction
{
    CAN_HIDE,
    CAN_OUT_HIDDEN_PLACE,
    CAN_ATTACK,
    CAN_SHOOT,
    NONE,
    CAN_OPEN_DOOR
}

public enum MyhaContactLightHiddenState
{
    MYHA_IS_NOT_ON_LIGHT,
    MYHA_IS_ON_LIGHT,
}
