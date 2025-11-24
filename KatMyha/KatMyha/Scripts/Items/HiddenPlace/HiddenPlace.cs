using Godot;
using KatrinaGame.Players;
using PrototipoMyha.Enemy;

public partial class HiddenPlace : Area2D
{
    private int TryToSearchCount = 1;
    public void _on_body_entered(Node2D body)
    {
        if (body is MyhaPlayer player)
            player.EnterHiddenPlace();

        if (body is EnemyBase enemy && enemy.EnemyResource.IsEnemyThatSearch)
        {
            if (true && TryToSearchCount > 0 && TryToSearchCount < 3)
            {
                TryToSearchCount++;
                return;
            }

            TryToSearchCount = 1;
            enemy.SetInHiddenPlace();
        }

    }

}
