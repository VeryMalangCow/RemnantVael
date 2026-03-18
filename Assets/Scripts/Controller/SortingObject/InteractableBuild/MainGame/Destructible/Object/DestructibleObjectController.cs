using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjectController : SortingObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Destructible Obj")]

    [Space(10)]
    [Header("=== Index")]
    [SerializeField] private int TypeIndex = 0;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        ThisSR.sprite = ResourceManager.instance.Get_RandomFieldObjSprite(
            StageManager.instance.targetStageID,
            TypeIndex);

        ThisSR.material = StageManager.instance.Get_CurrentStageData().MapMaterialClear[1];
    }

    #endregion

    #region Destruct

    public void Destruct()
    {
        UnitManager.instance.build_ExplImgGenerator.Expl_FieldObj(TargetObject.gameObject.transform.position);
        SoundManager.instance.Play_2D_SFX_Build("BreakFieldObj");
        Destroy(gameObject);
    }

    private void OnlyDestruct()
    {
        Destroy(gameObject);
    }

    #endregion

    #region 

    private void OnTriggerEnter2D(Collider2D _Col)
    {
        if (_Col.tag == "Wall" || _Col.tag == "FieldObj") 
            OnlyDestruct();
    }

    #endregion
}
