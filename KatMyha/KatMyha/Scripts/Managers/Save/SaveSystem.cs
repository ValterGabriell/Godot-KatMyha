using Godot;
using PrototipoMyha.Scripts.Utils.Objetos;
using PrototipoMyha.Utilidades;
using System;
using System.Text.Json;


public partial class SaveSystem : Node
{
    private const string SAVE_PATH = "user://savegame.json";

    public static SaveSystem SaveSystemInstance = null;
 

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

    public void SaveGame(LevelSaveData data)
    {
        try
        {
            string json = JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            using var file = Godot.FileAccess.Open(SAVE_PATH, Godot.FileAccess.ModeFlags.Write);

            if (file != null)
            {

                file.StoreString(json);
   
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

    public LevelSaveData LoadGame()
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
