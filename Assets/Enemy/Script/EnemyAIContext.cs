
public class EnemyAIContext
{
    public PlayerController player;
    public EnemyController enemy;

    // Constructor
    public EnemyAIContext(PlayerController player, EnemyController enemy)
    {
        this.player = player;
        this.enemy = enemy;
    }
}
