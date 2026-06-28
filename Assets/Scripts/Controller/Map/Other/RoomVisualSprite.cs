using UnityEngine;

// 스테이지 생성 시 적용하는 Build Sprite System
public class RoomVisualSprite : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer thisSr;
    [SerializeField] private bool isClearVisualMode;
    public SpriteRenderer ThisSr { get { return ThisSr; } }
    [SerializeField] private int spriteIndex;

#if UNITY_EDITOR
    public void SetData(int index)
    {
        thisSr = GetComponent<SpriteRenderer>();
        spriteIndex = index;
    }

#endif

    public void SetSprite(StageThemeSO stageThemeSO)
    {
        SpriteMaterial spriteMaterial = stageThemeSO.stageAllSprites[spriteIndex];
        thisSr.sprite = spriteMaterial.sprite;
        thisSr.material = stageThemeSO.mapMaterialClear[spriteMaterial.materialIndex];
    }
}
