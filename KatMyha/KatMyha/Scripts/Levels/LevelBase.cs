using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy;
using PrototipoMyha.Enemy;
using PrototipoMyha.Scripts.Managers;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public partial class LevelBase : Node2D
{
    [Export] private int LevelNumber { get; set; }
    private SaveSystem SaveSystem;
    

    public override void _Ready()
    {
        SaveSystem = SaveSystem.SaveSystemInstance;
        SaveSystem.SaveGame(LevelNumber);
    }
}
