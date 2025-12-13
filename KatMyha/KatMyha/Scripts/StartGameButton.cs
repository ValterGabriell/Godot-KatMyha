using Godot;
using PrototipoMyha;

public partial class StartGameButton : Button
{
    [Export] private CanvasLayer CanvasLayer;
    private SignalManager SignalManager;
    public override void _Ready()
    {
        this.SignalManager = SignalManager.Instance;
        this.Pressed += OnPressed;
    }

    private void OnPressed()
    {
        this.CanvasLayer.Visible = false;
        this.Disabled = true;
        SignalManager.StartGame.Invoke();
    }
}
