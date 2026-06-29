using UnityEngine;

// 스테이지 생성 시 적용하는 Build Sprite System
public class RoomVisualSprite : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer thisSr;
    [SerializeField] private bool isClearVisualMode;
    [SerializeField] private int spriteIndex;
    public SpriteRenderer ThisSr { get { return ThisSr; } }
    public bool IsClearVisualMode { get { return isClearVisualMode; } }

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
        SpriteMaterial spriteMaterial = stageThemeSO.stageAllSprites[spriteIndex];
        thisSr.sprite = spriteMaterial.sprite;
        thisSr.material = stageThemeSO.mapMaterialUnclear[spriteMaterial.materialIndex];
    }

    // 클리어
    public void SetClearMaterial(StageThemeSO stageThemeSO)
    {
        thisSr.material = stageThemeSO.mapMaterialClear[stageThemeSO.stageAllSprites[spriteIndex].materialIndex];
    }
}
