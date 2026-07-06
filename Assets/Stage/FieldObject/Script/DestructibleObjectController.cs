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

        SpriteMaterial spriteMaterial = StageManager.instance.GetRandomFieldObjSprite(typeIndex);
        thisSr.sprite = spriteMaterial.sprite;
        thisSr.material = StageManager.instance.stageObjectGenerator.currentStageThemeSO.mapMaterialClear[spriteMaterial.materialIndex];
    }

    #endregion

    #region Destruct

    public void Destruct()
    {
        VfxManager.instance.build_ExplImgGenerator.Expl_FieldObj(targetObject.gameObject.transform.position);
        SoundManager.instance.PlayBuildSfx(transform.position, "BreakFieldObj");
        Destroy(gameObject);
    }

    private void OnlyDestruct()
    {
        Destroy(gameObject);
    }

    #endregion

    #region Trigger

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Wall" || col.tag == "FieldObj") 
            OnlyDestruct();
    }

    #endregion
}
