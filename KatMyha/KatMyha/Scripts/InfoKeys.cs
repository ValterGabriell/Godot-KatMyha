using Godot;
using PrototipoMyha.Scripts.Managers;

public partial class InfoKeys : CanvasLayer
{
    private GameManager GameManager;
    [Export] private Timer Timer;
    [Export] private Label InfoLabel;

    public override void _Ready()
    {
        GameManager = GameManager.GetGameManagerInstance();
        this.ProcessMode = ProcessModeEnum.Always;
        this.Timer.Timeout += Timer_Timeout;
    }

    private void Timer_Timeout() => GameManager.SetShouldShowKeyInfo(false);



    public override void _Process(double delta)
    {
        this.Visible = GameManager.ShouldShowKeyInfo;
        this.InfoLabel.Text = Timer.TimeLeft.ToString("F2");
    }


}
