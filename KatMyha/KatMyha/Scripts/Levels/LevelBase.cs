using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy;
using PrototipoMyha.Enemy;
using PrototipoMyha.Scripts.Managers;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public partial class LevelBase : Node2D
{
    [Export] private int LevelNumber { get; set; }
    

    public override void _Ready()
    {
        var enemiesInScene = GetTree().GetNodesInGroup("enemy");
        var pyramdsInScene = GetTree().GetNodesInGroup("pyramd");
        var listaInimigosv2 = enemiesInScene.OfType<EnemyBaseV2>().ToList();
        var pyramdsInScenev = pyramdsInScene.OfType<KillFallPyramd>().ToList();
       
        var gameManagerInstance = GameManager.GetGameManagerInstance();
        gameManagerInstance.SetCurrentLevelNumber(LevelNumber);
        gameManagerInstance.SetCurrentLevelInitialData(enemyBaseV2s: listaInimigosv2, pyramdsInScenev);

        var saveSystem = SaveSystem.SaveSystemInstance;
        var levelSaveData = gameManagerInstance.GetCurrentLevelUpdatedData();
        if (levelSaveData != null)
        {
            saveSystem.SaveGame(levelSaveData);
        }
    }
}
