using System.Collections.Generic;
using UnityEngine;

public class NormalEnemyController : EnemyController
{
    [Space(20)]
    [Header("<><><><><> Normal")]

    [Space(10)]
    [Header("=== Unique Thing")]
    [SerializeField] private eEnemy ThisEnemyType;
    [SerializeField] private List<eEnemyPattern> ThisEnemyPatterns;

}
