using Godot;
using PrototipoMyha.Utilidades;
using System;

public partial class UIGameHabilities : MarginContainer
{
    private PlayerManager playerManager;
    private ItemList HabilitiesList;

    public override void _Ready()
    {
        playerManager = PlayerManager.GetPlayerGlobalInstance();
        playerManager.PlayerChangedHability += OnPlayerChangedHability;
        HabilitiesList = GetNode<ItemList>("ItemList");
        PopulateHabilitiesList();
    }

    private void OnPlayerChangedHability()
    {
        HabilitiesList.Clear();
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
        HabilitiesList.AddItem(hability.ToString());

        if (HabilitiesList.ItemCount > 0)
        {
            PlayerHabilityKey playerHabilityKey = playerManager.GetCurrentActivePlayerHability();

            if (IsCurrentActiveHability(hability, playerHabilityKey))
            {
                // Usa o índice real da lista (ItemCount - 1) ao invés do índice do enum
                int actualIndex = HabilitiesList.ItemCount - 1;
                HabilitiesList.Select(actualIndex);
            }

            if (ShouldSelectFirstHability(playerHabilityKey))
                HabilitiesList.Select(0);
        }
    }

    private static bool IsCurrentActiveHability(object hability, PlayerHabilityKey playerHabilityKey)
    {
        return hability.ToString() == playerHabilityKey.ToString();
    }

    private bool ShouldSelectFirstHability(PlayerHabilityKey playerHabilityKey)
    {
        return playerHabilityKey == PlayerHabilityKey.NONE || HabilitiesList.ItemCount < 2;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionPressed(nameof(EnumActionsInput.habilities_key_toggle)))
        {
            if (HabilitiesList.ItemCount == 0) return;
            int selectedIndex = HabilitiesList.GetSelectedItems()[0];
            string v = HabilitiesList.GetItemText((selectedIndex + 1) % HabilitiesList.ItemCount);
            HabilitiesList.Select((selectedIndex + 1) % HabilitiesList.ItemCount);
            playerManager.SetCurrentActivePlayerHability((PlayerHabilityKey)Enum.Parse(typeof(PlayerHabilityKey), v));
            GDLogger.LogYellow("Toggled Hability to: " + v);
        }
    }
}
