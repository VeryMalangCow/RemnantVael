
using System.Collections.Generic;
using UnityEngine;

public class BuildExplImgGenerator : ExplosionImgGenerator
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Build")]
    [SerializeField] private List<Sprite> SmokeSpriteList;

    #endregion

    #region Build

    // 생성, 파괴에 사용
    public void Expl_Build(Vector2 _SpawnPos)
    {
        Gen_ExplImg_Circle(
            new ExplState(
                new ExplState_Base(_SpawnPos, 24),
                new ExplState_Sprite(SmokeSpriteList, ResourceManager.instance.Get_ModuleMaterial("Explosion")),
                new ExplState_MoveAndScale(Vector2.zero, _Dis: 0.5f, _Scale: 1.0f, _Time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(Vector2.zero, _Dis: 1.2f, _Scale: 0.2f, _Time: 1.000f, 0.500f)));
    }

    #endregion


    #region Field Obj

    // 파괴에 사용
    public void Expl_FieldObj(Vector2 _SpawnPos)
    {
        Gen_ExplImg_Circle(
            new ExplState(
                new ExplState_Base(_SpawnPos, 8),
                new ExplState_Sprite(SmokeSpriteList, ResourceManager.instance.Get_ModuleMaterial("Explosion")),
                new ExplState_MoveAndScale(Vector2.zero, _Dis: 0.25f, _Scale: 1.0f, _Time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(Vector2.zero, _Dis: 0.6f, _Scale: 0.2f, _Time: 1.000f, 0.500f)));
    }

    #endregion
}
