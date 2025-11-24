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

public enum LightHiddenState
{
    LIGHT_HIDDEN,
    LIGHT_NOT_HIDDEN,
}
