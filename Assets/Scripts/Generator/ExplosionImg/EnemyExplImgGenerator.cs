using System.Collections.Generic;
using UnityEngine;

public class EnemyExplImgGenerator : ExplosionImgGenerator
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]
    [SerializeField] private List<Sprite> smokeSpriteList;

    private Material explosionMaterial;

    #endregion
    private void Start()
    {
        explosionMaterial = StaticResourceManager.instance.ExplosionReso.enemyExplosionMaterial;
    }


    #region Enemy

    // 생성, 파괴에 사용
    public void Expl_Enemy(Vector2 spawnPos)
    {
        Gen_ExplImg_Circle(
            new ExplState(
                new ExplState_Base(spawnPos, 16),
                new ExplState_Sprite(smokeSpriteList, explosionMaterial),
                new ExplState_MoveAndScale(Vector2.zero, dis: 0.15f, scale: 0.6f, time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(Vector2.zero, dis: 0.75f, scale: 0.2f, time: 0.750f, 0.250f)));
    }

    // 적이 발사
    public void Expl_Enemy_Shoot(Vector2 spawnPos, Vector2 dir, int amount)
    {
        Gen_ExplImg_Sector(
            new ExplState(
                new ExplState_Base(spawnPos, amount),
                new ExplState_Sprite(smokeSpriteList, explosionMaterial),
                new ExplState_MoveAndScale(dir, dis: 0.2f, scale: 0.8f, time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(dir, dis: 0.5f, scale: 0.4f, time: 0.750f, 0.250f)),
            dir, 45f);
    }

    // 적의 오브젝트 파괴
    public void Expl_Enemy_ObjectDestroy(Vector2 spawnPos, int amount)
    {
        Gen_ExplImg_Circle(
            new ExplState(
                new ExplState_Base(spawnPos, amount),
                new ExplState_Sprite(smokeSpriteList, explosionMaterial),
                new ExplState_MoveAndScale(Vector2.zero, dis: 0.3f, scale: 0.6f, time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(Vector2.zero, dis: 0.4f, scale: 0.3f, time: 0.750f, 0.250f)));
    }

    #endregion
}
