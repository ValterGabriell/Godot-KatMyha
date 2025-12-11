using Godot;
using KatMyha.Scripts.Enemies.DroneEnemy;
using KatMyha.Scripts.Items.KillLight;
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

            //GDLogger.LogGreen("Save serializado com sucesso!");
            //GDLogger.Log(json);

            using var file = Godot.FileAccess.Open(SAVE_PATH, Godot.FileAccess.ModeFlags.Write);

            if (file != null)
            {
                bool isSaveSuccessful = file.StoreString(json);
                //GDLogger.LogGreen("Jogo salvo com: " + isSaveSuccessful);
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
        var fall_trapInScene = GetTree().GetNodesInGroup("fall_trap");
        var pyramdsInScenev = pyramdsInScene.OfType<KillFallPyramd>().ToList();
        var fall_trapInScenev = fall_trapInScene.OfType<FallTrap>().ToList();

        var gameManagerInstance = GameManager.GetGameManagerInstance();
        var playerManagerInstance = PlayerManager.GetPlayerGlobalInstance();

        gameManagerInstance.SetCurrentLevelNumber(LevelNumber);
        gameManagerInstance.SetCurrentLevelInitialData(pyramdsInScenev, fall_trapInScenev);

        var saveSystem = SaveSystem.SaveSystemInstance;
        var levelSaveData = gameManagerInstance.GetCurrentLevelUpdatedData();

        levelSaveData.PlayerSubphaseKeysObtained = playerManagerInstance.GetObtainedKeys();
        levelSaveData.PlayerHabilitiesUnlocked = playerManagerInstance.GetUnlockedPlayerHabilities();

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

        var playerManagerInstance = PlayerManager.GetPlayerGlobalInstance();
        playerManagerInstance.UpdatePlayerPosition(loadedPosition);
        playerManagerInstance.BasePlayer.SetState(PlayerState.IDLE);

        playerManagerInstance.SetObtainedKeys(saveData.PlayerSubphaseKeysObtained);
        playerManagerInstance.SetUnlockedPlayerHabilities(saveData.PlayerHabilitiesUnlocked);


        var enemiesInScene = GetTree().GetNodesInGroup("enemy");
        var pyramdInScene = GetTree().GetNodesInGroup("pyramd");
        var fallTrapInScene = GetTree().GetNodesInGroup("fall_trap");
        var alertsInScene = GetTree().GetNodesInGroup(EnumGroups.AlertSprite.ToString());
        var alertSoundsInScene = GetTree().GetNodesInGroup("alert_sound");



        foreach (var item in fallTrapInScene)
            item.QueueFree();


        foreach (var item in alertsInScene)
            item.QueueFree();

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

        foreach (var item in saveData.FallTrapKillList)
        {
            var fall = fallTrapInScene
                .OfType<FallTrap>()
                .FirstOrDefault(e => e.InstanceID == item.InstanceID);



            if (fall != null)
            {

                var newFall = (FallTrap)GD.Load<PackedScene>(fall.SceneFilePath).Instantiate();
                newFall.InstanceID = item.InstanceID;

                // IMPORTANTE: Preparar para load ANTES de adicionar à cena
                newFall.PrepareForLoad();

                if (item.MarkerData_A != null && item.MarkerData_B != null)
                {
                    GDLogger.LogYellow("Carregando FallTrap ID: " + item.InstanceID.ToString());
                    GDLogger.LogYellow("Carregando FallTrap PosX: " + item.PositionX.ToString());
                    GDLogger.LogYellow("Carregando FallTrap PosY: " + item.PositionY);
                    GDLogger.LogYellow("Carregando FallTrap ScenePath: " + fall.SceneFilePath);
                    GDLogger.LogYellow("Carregando FallTrap MarkerA: " + item.MarkerData_A.ToString());
                    GDLogger.LogYellow("Carregando FallTrap MarkerB: " + item.MarkerData_B.ToString());
                    GDLogger.LogYellow("Carregando FallTrap IsTargetingMarkerA: " + item.IsTargetingMarkerA);
                }

                GetTree().CurrentScene.AddChild(newFall);

                // IMPORTANTE: Definir posição DEPOIS de adicionar à cena
                newFall.GlobalPosition = new Vector2(item.PositionX, item.PositionY);

                if (item.MarkerData_A != null && item.MarkerData_B != null)
                {
                    GDLogger.LogYellow("===== CARREGANDO FALLTRAP =====");
                    GDLogger.LogYellow("ID: " + item.InstanceID.ToString());
                    GDLogger.LogYellow("Objeto Pos: (" + item.PositionX + ", " + item.PositionY + ")");
                    GDLogger.LogYellow("ScenePath: " + fall.SceneFilePath);
                    GDLogger.LogYellow("MarkerA: " + item.MarkerData_A.ToString());
                    GDLogger.LogYellow("MarkerB: " + item.MarkerData_B.ToString());
                    GDLogger.LogYellow("IsTargetingMarkerA: " + item.IsTargetingMarkerA);

                    // Configurar marcadores
                    newFall.SetMarkerPositions(
                        item.MarkerData_A.PositionX,
                        item.MarkerData_A.PositionY,
                        item.MarkerData_B.PositionX,
                        item.MarkerData_B.PositionY
                    );

                    // Configurar target
                    newFall.SetTargetMarker(item.IsTargetingMarkerA);

                    GDLogger.LogYellow("===== FIM CARREGAMENTO =====");
                }
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
