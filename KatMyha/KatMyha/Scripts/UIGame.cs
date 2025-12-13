using Godot;
using PrototipoMyha;

public partial class UIGame : CanvasLayer
{
    [Signal] public delegate void UIChangeVisibilityEventHandler(bool isVisible);
    [Export] public Sprite2D Sprite2D { get; set; }
    [Export] public Label Timer { get; set; }
    [Export] public Label Keys { get; set; }
    private double _elapsedTime = 0.0;
    private bool _timerActive = false;
    private SignalManager SignalManager;
    private PlayerManager PlayerManager;

    private Texture2D Texture2DInVisible = GD.Load<Texture2D>("res://Assets/Sprites/Itens/UIMyhaInVisible.png");
    private Texture2D Texture2DVisible = GD.Load<Texture2D>("res://Assets/Sprites/Itens/UiMyhaVisible.png");

    public override void _Ready()
    {
        PlayerManager = PlayerManager.GetPlayerGlobalInstance();
        SignalManager = SignalManager.Instance;
        SignalManager.StartGame += OnStartButtonPressed;
        UIChangeVisibility += UIGame_UIChangeVisibility;
    }

    public override void _Process(double delta)
    {
        if (_timerActive && Timer != null)
        {
            _elapsedTime += delta;
            Timer.Text = _elapsedTime.ToString("F2");
        }

        Keys.Text = "Chaves Coletadas: " + PlayerManager.GetCountOfUnlockedKeys().ToString();
    }

    private void OnStartButtonPressed()
    {
        _elapsedTime = 0.0;
        _timerActive = true;
    }

    private void UIGame_UIChangeVisibility(bool isVisible)
    {
        if (isVisible)
            Sprite2D.Texture = Texture2DVisible;
        else
            Sprite2D.Texture = Texture2DInVisible;
    }
}
