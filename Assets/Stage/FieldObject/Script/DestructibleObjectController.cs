using UnityEngine;

public class DestructibleObjectController : SortingObjectController
{
    [Space(20)]
    [Header("<><><><><> Destructible Obj")]

    [Space(10)]
    [Header("=== Index")]
    [SerializeField] private int typeIndex = 0;

    public void SetVisual()
    {
        SpriteMaterial spriteMaterial = StageManager.instance.GetRandomFieldObjSprite(typeIndex);
        thisSr.sprite = spriteMaterial.sprite;
        thisSr.material = StageManager.instance.stageObjectGenerator.currentStageThemeSO.mapMaterialClear[spriteMaterial.materialIndex];
    }

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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Wall" || col.tag == "FieldObj") 
            OnlyDestruct();
    }

}
