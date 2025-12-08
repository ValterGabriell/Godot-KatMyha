using Godot;
using System;

public partial class BaseDialogBox : CanvasLayer
{
    private string LabelText = "Default Dialog Text";
    private Texture2D Sprite = null!;
    public event Action DialogClosed;


    public string DialogText
    {
        get => LabelText;
        set
        {
            LabelText = value;
        }
    }

    public Texture2D DialogSprite
    {
        get => Sprite;
        set
        {
            Sprite = value;
        }
    }

    public override void _Ready()
    {
        var label = this.GetNode<MarginContainer>("MarginContainer").GetNode<MarginContainer>("MarginContainer").GetNode<HBoxContainer>("HBoxContainer").GetNode<Label>("TextLabel");
        var sprite = this.GetNode<MarginContainer>("MarginContainer").GetNode<MarginContainer>("MarginContainer").GetNode<Sprite2D>("Sprite2D");
        label.Text = LabelText;
        sprite.Texture = Sprite;
        this.ProcessMode = ProcessModeEnum.Always;
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
