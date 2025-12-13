using Godot;
using PrototipoMyha;

public enum EnumSpecialHorizontalPlataform
{
    FALLING_WHEN_GRAB_FIRST_KEY = 0,
}

public partial class HorizontalPlataform : StaticBody2D
{
    [Export] private bool HasToFall { get; set; }
    [Export] private float TimeToFall { get; set; } = 2f;

    [ExportGroup("Special Plataform Settings")]
    [Export] private bool IsSpecialPlatform { get; set; } = false;
    [Export] private EnumSpecialHorizontalPlataform SpecialType { get; set; }


    private bool MyhaIsOnPlatform = false;
    private Tween blinkTween;
    private PlayerManager PlayerManager;

    public override void _Ready()
    {
        PlayerManager = PlayerManager.GetPlayerGlobalInstance();
    }

    public void _on_area_2d_body_entered(Node2D node2D)
    {
        if (node2D.IsInGroup(EnumGroups.player.ToString()) && HasToFall)
        {
            MyhaIsOnPlatform = true;
            StartBlinking();
            GetTree().CreateTimer(TimeToFall).Timeout += () =>
            {
                this.Visible = false;
                this.GetNode<Area2D>("Area2D").Monitoring = false;
                this.GetNode<Area2D>("Area2D").Monitorable = false;
            };

        }

        if (IsSpecialPlatform
               && SpecialType == EnumSpecialHorizontalPlataform.FALLING_WHEN_GRAB_FIRST_KEY
               && PlayerManager.IsPlayerSubphaseKeyUnlocked(PlayerSubphaseKey.SUBFASE_1))
        {
            StartBlinking();
            GetTree().CreateTimer(TimeToFall).Timeout += () =>
            {
                this.Visible = false;
                this.GetNode<Area2D>("Area2D").Monitoring = false;
                this.GetNode<Area2D>("Area2D").Monitorable = false;
            };
        }
    }

    private void StartBlinking()
    {
        if (blinkTween != null && blinkTween.IsValid())
            return;

        blinkTween = CreateTween();
        blinkTween.SetLoops();
        blinkTween.TweenProperty(this, "modulate:a", 0.0f, 0.2);
        blinkTween.TweenProperty(this, "modulate:a", 1.0f, 0.2);
    }

    internal void ResetPlataform()
    {
        this.Visible = true;
        this.GetNode<Area2D>("Area2D").Monitoring = true;
        this.GetNode<Area2D>("Area2D").Monitorable = true;

        if (blinkTween != null && blinkTween.IsValid())
        {
            blinkTween.Kill();
            blinkTween = null;
        }

        this.Modulate = new Color(this.Modulate.R, this.Modulate.G, this.Modulate.B, 1.0f);
    }
}
