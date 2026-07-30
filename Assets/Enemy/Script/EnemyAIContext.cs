
using UnityEngine;

public class EnemyAIContext
{
    public PlayerController player;
    public EnemyController enemy;
    public int wallLayer;

    // Constructor
    public EnemyAIContext(PlayerController player, EnemyController enemy)
    {
        this.player = player;
        this.enemy = enemy;
        this.wallLayer = LayerMask.GetMask("Wall");
    }
}
