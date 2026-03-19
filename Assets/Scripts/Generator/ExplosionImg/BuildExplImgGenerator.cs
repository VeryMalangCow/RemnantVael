
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
                new ExplState_MoveAndScale(Vector2.zero, dis: 0.5f, scale: 1.0f, time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(Vector2.zero, dis: 1.2f, scale: 0.2f, time: 1.000f, 0.500f)));
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
                new ExplState_MoveAndScale(Vector2.zero, dis: 0.25f, scale: 1.0f, time: 0.075f, 0.025f),
                new ExplState_MoveAndScale(Vector2.zero, dis: 0.6f, scale: 0.2f, time: 1.000f, 0.500f)));
    }

    #endregion
}
