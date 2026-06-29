using UnityEngine;

public class BuildPassageSpriteController : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer thisSr;
    [SerializeField] private int spriteIndex;
    [SerializeField] private bool isBeforeMap = true;
    public bool IsBeforeMap { get { return isBeforeMap; } }

#if UNITY_EDITOR
    public void SetData(int index)
    {
        thisSr = GetComponent<SpriteRenderer>();
        spriteIndex = index;
    }

#endif

    // 초기 설정
    public void SetSprite(StageThemeSO stageThemeSO)
    {
        Vector2 tileSize = thisSr.size;
        SpriteMaterial spriteMaterial = stageThemeSO.stageAllSprites[spriteIndex];
        thisSr.sprite = spriteMaterial.sprite;
        thisSr.material = stageThemeSO.mapMaterialUnclear[spriteMaterial.materialIndex];
        thisSr.size = tileSize;
    }
}
