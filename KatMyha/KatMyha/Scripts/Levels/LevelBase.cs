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
        var listaInimigos = enemiesInScene.OfType<EnemyBase>().ToList();
        var listaInimigosv2 = enemiesInScene.OfType<EnemyBaseV2>().ToList();
       
        var gameManagerInstance = GameManager.GetGameManagerInstance();
        gameManagerInstance.SetCurrentLevelNumber(LevelNumber);
        gameManagerInstance.SetCurrentLevelInitialData(enemies: listaInimigos, enemyBaseV2s: listaInimigosv2);
    }
}
