using UnityEngine;

public class RoomPassageMiddleVisualSprite : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer thisSr;
    [SerializeField] private int spriteIndex;
    public SpriteRenderer ThisSr { get { return ThisSr; } }

#if UNITY_EDITOR
    public void SetData(int index)
    {
        thisSr = GetComponent<SpriteRenderer>();
        spriteIndex = index;
    }

#endif
    // 초기 설정
    public void SetSprite(StagePassageThemeSO passageStageThemeSO)
    {
        Vector2 tileSize = thisSr.size;
        thisSr.sprite = passageStageThemeSO.sprites[spriteIndex];
        thisSr.material = passageStageThemeSO.mapMaterial;
        thisSr.size = tileSize;
    }

}
