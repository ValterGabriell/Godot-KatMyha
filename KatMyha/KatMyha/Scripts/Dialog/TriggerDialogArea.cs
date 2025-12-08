using Godot;
using KatMyha.Scripts.Dialog;
using KatrinaGame.Players;
using System;
using System.Collections.Generic;
using System.Linq;

public enum ActionsOfBaseConfigScene
{
    NONE,
    P01_OPEN_FIRST_DOOR,
}

public partial class TriggerDialogArea : Area2D
{
    [Export] private string[] Dialogs = null!;
    [Export] private Texture2D[] DialogsTextures = null!;
    [Export] private bool QueueFreeAtEnd = true;
    [Export] private PackedScene DialogScene;
    [Export] private bool ShouldPauseGameWhenItAppears = false;

    [ExportGroup("Scene Config")]
    [ExportCategory("Scene Config")]
    [Export] private BaseConfigScenesLevel? PSomething_SceneBase;
    [Export] private ActionsOfBaseConfigScene ActionsOfBaseConfigScene;


    private int _currentDialogIndex = 0;
    private List<DtoBaseDialog> dialogScenes = [];

    public override void _Ready()
    {
        this.BodyEntered += OnBodyEntered;
        this.ProcessMode = ProcessModeEnum.Always;

    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is MyhaPlayer player)
        {
            if (player.PlayeCanMoveCamera) return; // Use a propriedade correta (PlayeCanMoveCamera)
            if (Dialogs.Count() == 0 || DialogsTextures.Count() == 0) return;

            // Garante que o player pode ser pausado antes de pausar o jogo
            if (ShouldPauseGameWhenItAppears)
            {
                player.ProcessMode = ProcessModeEnum.Pausable;
                GetTree().Paused = true;
            }

            for (int i = 0; i < Dialogs.Length; i++)
            {
                var dialogText = Dialogs[i];
                var dialogTexture = DialogsTextures.Length > i ? DialogsTextures[i] : null;
                var dtoDialog = new DtoBaseDialog(dialogText, dialogTexture);
                dialogScenes.Add(dtoDialog);
            }

            for (int i = 0; i < dialogScenes.Count - 1; i++)
            {
                dialogScenes[i].SetNext(dialogScenes[i + 1]);
            }

            ShowNextDialog();
        }
    }
    private void ShowNextDialog()
    {
        if (_currentDialogIndex >= dialogScenes.Count)
        {
            if (QueueFreeAtEnd) this.QueueFree();
            if (ShouldPauseGameWhenItAppears) GetTree().Paused = false;
            TriggerActionsOfTerminalWhenItHasSomeone();
            return;
        }


        var currentDialog = dialogScenes[_currentDialogIndex];
        var dialogBox = DialogScene.Instantiate<BaseDialogBox>();
        dialogBox.DialogText = currentDialog.GetDialogText();
        dialogBox.DialogSprite = currentDialog.GetCurrentSprite();
        dialogBox.DialogClosed += OnDialogClosed;
        GetTree().CurrentScene.AddChild(dialogBox);
    }

    private void TriggerActionsOfTerminalWhenItHasSomeone()
    {
        if (PSomething_SceneBase != null)
        {
            var actionToTrigger = ActionsOfBaseConfigScene switch
            {
                ActionsOfBaseConfigScene.P01_OPEN_FIRST_DOOR => PSomething_SceneBase.OnP01OpenFirstDoor(),
                _ => () => { }
            };
            actionToTrigger.Invoke();
        }
    }

    private void OnDialogClosed()
    {
        _currentDialogIndex++;
        ShowNextDialog();
    }
}
