using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIContext
{
    public PlayerController player;
    public EnemyController enemy;
    public int wallLayer;

    public Dictionary<Type, EnemyModule> modules;

    // Constructor
    public EnemyAIContext(PlayerController player, EnemyController enemy)
    {
        this.player = player;
        this.enemy = enemy;
        this.wallLayer = LayerMask.GetMask("Wall");

        EnemyModule[] allModule = enemy.GetComponents<EnemyModule>();
        int length = allModule.Length;
        modules = new Dictionary<Type, EnemyModule>(length);
        for (int i = 0; i < length; i++)
        {
            EnemyModule module = allModule[i];
            modules.TryAdd(module.GetType(), module);
        }
    }

    public T GetModule<T>() where T : EnemyModule
    {
        modules.TryGetValue(typeof(T), out EnemyModule module);
        return module as T;
    }
}