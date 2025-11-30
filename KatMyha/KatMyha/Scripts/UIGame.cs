using Godot;
using PrototipoMyha;
using System;

public partial class UIGame : Control
{
    [Signal] public delegate void UIChangeVisibilityEventHandler(bool isVisible);
    [Export] public Label LabelVisibility { get; set; }
    public override void _Ready()
    {
        UIChangeVisibility += UIGame_UIChangeVisibility;
    }

    private void UIGame_UIChangeVisibility(bool isVisible)
    {
        if (isVisible)
        {
            LabelVisibility.Text = "Myha está visivel!";
        }
        else
        {
            LabelVisibility.Text = "Você está escondida!!";
        }
    }
}
