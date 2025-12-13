using Godot;
using KatrinaGame.Players;

public partial class HiddenPlace : Area2D
{
    private int TryToSearchCount = 1;
    public void _on_body_entered(Node2D body)
    {
        if (body is MyhaPlayer player)
            player.EnterHiddenPlace(this.GlobalPosition);

    }

}
