using Godot;
using KatMyha.Scripts.Dialog;
using KatrinaGame.Players;
using PrototipoMyha.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class TriggerDialogArea : Area2D
{
    [Export] private string[] Dialogs = null!;
    [Export] private Texture2D[] DialogsTextures = null!;
    [Export] private bool QueueFreeAtEnd = true;
    private int _currentDialogIndex = 0;
    private List<DtoBaseDialog> dialogScenes = [];
    public override void _Ready()
    {
        this.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is MyhaPlayer)
        {
            if (Dialogs.Count() == 0 || DialogsTextures.Count() == 0) return;
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
            if(QueueFreeAtEnd) this.QueueFree(); 
            return;
        }
            

        var currentDialog = dialogScenes[_currentDialogIndex];
        var dialogBox = GD.Load<PackedScene>("res://Scenes/Dialogs/BaseDialogBox.tscn").Instantiate<BaseDialogBox>();
        dialogBox.DialogText = currentDialog.GetDialogText();
        dialogBox.DialogSprite = currentDialog.GetCurrentSprite();
        dialogBox.DialogClosed += OnDialogClosed;
        GetTree().CurrentScene.AddChild(dialogBox);
    }

    private void OnDialogClosed()
    {
        _currentDialogIndex++;
        ShowNextDialog();
    }
}
