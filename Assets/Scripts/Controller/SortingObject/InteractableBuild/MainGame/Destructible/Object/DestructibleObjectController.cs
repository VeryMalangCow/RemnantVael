using UnityEngine;

public class DestructibleObjectController : SortingObjectController
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Destructible Obj")]

    [Space(10)]
    [Header("=== Index")]
    [SerializeField] private int typeIndex = 0;

    #endregion

    #region Offset

    protected override void Offset()
    {
        base.Offset();

        thisSr.sprite = ResourceManager.instance.Get_RandomFieldObjSprite(
            StageManager.instance.targetStageID,
            typeIndex);

        thisSr.material = StageManager.instance.Get_CurrentStageData().mapMaterialClear[1];
    }

    #endregion

    #region Destruct

    public void Destruct()
    {
        UnitManager.instance.build_ExplImgGenerator.Expl_FieldObj(targetObject.gameObject.transform.position);
        SoundManager.instance.Play_2D_SFX_Build("BreakFieldObj");
        Destroy(gameObject);
    }

    private void OnlyDestruct()
    {
        Destroy(gameObject);
    }

    #endregion

    #region 

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Wall" || col.tag == "FieldObj") 
            OnlyDestruct();
    }

    #endregion
}
