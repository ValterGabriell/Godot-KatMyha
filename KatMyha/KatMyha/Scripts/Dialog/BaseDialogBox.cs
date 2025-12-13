using Godot;
using System;

public partial class BaseDialogBox : CanvasLayer
{
    private string _dialogText = "Default Dialog Text";
    private Texture2D _sprite = null!;

    public event Action DialogClosed;

    public string DialogText
    {
        get => _dialogText;
        set => _dialogText = value;
    }

    public Texture2D DialogSprite
    {
        get => _sprite;
        set => _sprite = value;
    }

    public override void _Ready()
    {
        var root = GetNode<MarginContainer>("MarginContainer/MarginContainer");

        var panelText = root.GetNode<Panel>("Panel2");
        var textLabel = panelText.GetNode<RichTextLabel>("TextLabel");
        var sprite = root.GetNode<Sprite2D>("Sprite2D");



        // 🧠 CONFIGURAÇÃO CORRETA DO TEXTO
        textLabel.Text = _dialogText;
        textLabel.AutowrapMode = TextServer.AutowrapMode.Word;
        textLabel.ScrollActive = false;
        textLabel.FitContent = true;
        textLabel.ClipContents = true;

        // 🧱 Layout correto
        textLabel.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        textLabel.SizeFlagsVertical = Control.SizeFlags.ExpandFill;

        // 🎭 Sprite do personagem
        sprite.Texture = _sprite;

        ProcessMode = ProcessModeEnum.Always;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_accept"))
        {
            DialogClosed?.Invoke();
            QueueFree();
        }
    }
}
