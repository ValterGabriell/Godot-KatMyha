using Godot;
using System;

public partial class UIGameHabilities : MarginContainer
{
    private PlayerManager playerManager;
    private Sprite2D HabilitiesIcon;
    [Export] private Label Aim;
    [Export] private Label Shoot;
    private Tween blinkTween;
    public override void _Ready()
    {
        playerManager = PlayerManager.GetPlayerGlobalInstance();
        playerManager.PlayerChangedHability += OnPlayerChangedHability;
        HabilitiesIcon = GetNode<Sprite2D>("Sprite2D");
        PopulateHabilitiesList();
    }

    public override void _Process(double delta)
    {
        if (Aim.Visible && Shoot.Visible)
        {
            GetTree().CreateTimer(5.0).Timeout += () =>
            {
                StartBlinking();
            };

            GetTree().CreateTimer(10.0).Timeout += () =>
            {
                Aim.Visible = false;
                Shoot.Visible = false;
                if (blinkTween != null && blinkTween.IsValid())
                {
                    blinkTween.Kill();
                }
            };

        }
    }

    private void OnPlayerChangedHability()
    {
        HabilitiesIcon.Visible = false;
        /*
         Esse for foi feito dessa forma porque esse mesmo metodo é chamado quando usa o TAB pra trocar
        habilidade, o que gerava inconformidade porque a lista era apagada e refeita, mantendo a selecao sempre
        no primeiro item. Entao agora, ao trocar a habilidade, apenas o item selecionado é alterado.
         */
        if (playerManager.GetCountOfUnlockedHabilities() == 0) return;
        PopulateHabilitiesList();
    }

    private void PopulateHabilitiesList()
    {
        for (int i = 0; i < Enum.GetValues(typeof(PlayerHabilityKey)).Length; i++)
        {
            var hability = (PlayerHabilityKey)i;
            if (playerManager.IsPlayerHabilityUnlocked((PlayerHabilityKey)hability))
                AddHabilityToList(hability);
        }
    }

    private void AddHabilityToList(object hability)
    {
        HabilitiesIcon.Visible = true;
        Aim.Visible = true;
        Shoot.Visible = true;
        Aim.Text = "Mire com o botao direito do mouse.";
        Shoot.Text = "Atire com o botao esquerdo do mouse.";
    }

    private void StartBlinking()
    {
        if (blinkTween != null && blinkTween.IsValid())
            return;

        blinkTween = CreateTween();
        blinkTween.SetLoops();
        blinkTween.TweenProperty(Aim, "modulate:a", 0.0f, 0.2);
        blinkTween.TweenProperty(Shoot, "modulate:a", 0.0f, 0.2);
        blinkTween.TweenProperty(Aim, "modulate:a", 1.0f, 0.2);
        blinkTween.TweenProperty(Shoot, "modulate:a", 1.0f, 0.2);
    }


}
