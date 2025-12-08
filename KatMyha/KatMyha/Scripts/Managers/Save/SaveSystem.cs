using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy;
using PrototipoMyha;
using PrototipoMyha.Enemy.States;
using PrototipoMyha.Player.StateManager;
using PrototipoMyha.Scripts.Managers;
using PrototipoMyha.Scripts.Utils.Objetos;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;


public partial class SaveSystem : Node
{
    private const string SAVE_PATH = "user://savegame.json";

    public static SaveSystem SaveSystemInstance = null;

    private struct EnemyLoadData
    {
        public EnemyBaseV2 Enemy;
        public Vector2 Position;
        public Guid InstanceId;
        public EnumEnemyState State;
    }

    private readonly List<EnemyLoadData> _pendingEnemies = new();

    public override void _Ready()
    {
        if (SaveSystemInstance == null)
        {
            SaveSystemInstance = this;
        }
        else
        {
            QueueFree();
        }
    }

    private void SaveGame(LevelSaveData data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            GDLogger.LogGreen("Save serializado com sucesso!");
            GDLogger.Log(json);

            using var file = Godot.FileAccess.Open(SAVE_PATH, Godot.FileAccess.ModeFlags.Write);

            if (file != null)
            {
                bool isSaveSuccessful = file.StoreString(json);
                GDLogger.LogGreen("Jogo salvo com: " + isSaveSuccessful);
            }
            else
            {
                GD.PrintErr("Erro ao salvar o jogo!");
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Erro ao serializar o save: {ex.Message}");
        }
    }

    public void SaveGame(int LevelNumber)
    {
        var pyramdsInScene = GetTree().GetNodesInGroup("pyramd");
        var pyramdsInScenev = pyramdsInScene.OfType<KillFallPyramd>().ToList();

        var gameManagerInstance = GameManager.GetGameManagerInstance();
        gameManagerInstance.SetCurrentLevelNumber(LevelNumber);
        gameManagerInstance.SetCurrentLevelInitialData(pyramdsInScenev);

        var saveSystem = SaveSystem.SaveSystemInstance;
        var levelSaveData = gameManagerInstance.GetCurrentLevelUpdatedData();
        if (levelSaveData != null)
        {
            saveSystem.SaveGame(levelSaveData);
        }
    }

    private LevelSaveData LoadGame()
    {
        if (!Godot.FileAccess.FileExists(SAVE_PATH))
        {
            GD.Print("Arquivo de save não encontrado!");
            return null;
        }

        using var file = Godot.FileAccess.Open(SAVE_PATH, Godot.FileAccess.ModeFlags.Read);

        if (file != null)
        {
            string json = file.GetAsText();
            LevelSaveData data = JsonSerializer.Deserialize<LevelSaveData>(json);

            return data;
        }

        GD.PrintErr("Erro ao carregar o jogo!");

        return null;
    }

    public void Load()
    {
        var saveData = LoadGame();

        Vector2 loadedPosition = new(saveData.PlayerPosition_X_OnLevel, saveData.PlayerPosition_Y_OnLevel);

        SignalManager.Instance.EmitSignal(nameof(SignalManager.GameLoaded), loadedPosition);

        var instanceManager = PlayerManager.GetPlayerGlobalInstance();
        instanceManager.UpdatePlayerPosition(loadedPosition);
        instanceManager.BasePlayer.SetState(PlayerState.IDLE);

        var enemiesInScene = GetTree().GetNodesInGroup("enemy");
        var pyramdInScene = GetTree().GetNodesInGroup("pyramd");
        var alertsInScene = GetTree().GetNodesInGroup(EnumGroups.AlertSprite.ToString());
        var alertSoundsInScene = GetTree().GetNodesInGroup("alert_sound");

        foreach (var item in alertsInScene)
        {
            item.QueueFree();
        }

        foreach (var item in alertSoundsInScene)
        {
            if (item is Node node)
            {
                node.QueueFree();
            }
        }

        foreach (var enemy in enemiesInScene)
        {
            if (enemy is Node node)
            {
                node.QueueFree();
            }
        }



        _pendingEnemies.Clear();



        foreach (var item in saveData.PyramdsFallKill)
        {
            var pyramed = pyramdInScene
                .OfType<KillFallPyramd>()
                .FirstOrDefault(e => e.InstanceID == item.InstanceID);

            if (pyramed != null)
            {
                var newPyramed = (KillFallPyramd)GD.Load<PackedScene>("res://Scenes/Items/Itens/KillFallPyramd.tscn").Instantiate();
                newPyramed.InstanceID = item.InstanceID;
                newPyramed.GlobalPosition = new Vector2(item.PositionX, item.PositionY);
                GetTree().CurrentScene.AddChild(newPyramed);
            }
        }
    }


    public bool SaveExists()
    {
        return Godot.FileAccess.FileExists(SAVE_PATH);
    }

    public void DeleteSave()
    {
        if (Godot.FileAccess.FileExists(SAVE_PATH))
        {
            Godot.DirAccess.RemoveAbsolute(SAVE_PATH);
            GD.Print("Save deletado!");
        }
    }
}
