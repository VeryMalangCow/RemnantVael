using System.Collections.Generic;
using UnityEngine;

public class EnemyExplImgGenerator : ExplosionImgGenerator
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Enemy")]
    [SerializeField] private List<Sprite> SmokeSpriteList;

    #endregion

    #region Enemy

    // 생성, 파괴에 사용
    public void Expl_Enemy(Vector2 _SpawnPos)
    {
        Gen_ExplImg_Circle(
            new ExplState(
                new ExplState_Base(_SpawnPos, 16),
                new ExplState_Sprite(SmokeSpriteList, UnitManager.Instance.EnemyM_000_Explosion),
                new ExplState_MoveAndScale(Vector2.zero, _Dis: 0.15f, _Scale: 0.6f, _Time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(Vector2.zero, _Dis: 0.75f, _Scale: 0.2f, _Time: 0.750f, 0.250f)));
    }

    // 적이 발사
    public void Expl_Enemy_Shoot(Vector2 _SpawnPos, Vector2 _Dir, int _Amount)
    {
        Gen_ExplImg_Sector(
            new ExplState(
                new ExplState_Base(_SpawnPos, _Amount),
                new ExplState_Sprite(SmokeSpriteList, UnitManager.Instance.EnemyM_000_Explosion),
                new ExplState_MoveAndScale(_Dir, _Dis: 0.2f, _Scale: 0.8f, _Time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(_Dir, _Dis: 0.5f, _Scale: 0.4f, _Time: 0.750f, 0.250f)),
            _Dir, 45f);
    }

    // 적의 오브젝트 파괴
    public void Expl_Enemy_ObjectDestroy(Vector2 _SpawnPos, int _Amount)
    {
        Gen_ExplImg_Circle(
            new ExplState(
                new ExplState_Base(_SpawnPos, _Amount),
                new ExplState_Sprite(SmokeSpriteList, UnitManager.Instance.EnemyM_000_Explosion),
                new ExplState_MoveAndScale(Vector2.zero, _Dis: 0.3f, _Scale: 0.6f, _Time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(Vector2.zero, _Dis: 0.4f, _Scale: 0.3f, _Time: 0.750f, 0.250f)));
    }

    #endregion
}
