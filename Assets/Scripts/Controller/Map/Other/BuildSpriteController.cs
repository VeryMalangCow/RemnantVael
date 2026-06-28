using Unity.VisualScripting;
using UnityEngine;

// 스테이지 생성 시 적용하는 Build Sprite System
public class BuildSpriteController : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer thisSr;
    public SpriteRenderer ThisSr { get { return ThisSr; } }
    [SerializeField] private int spriteIndex;

#if UNITY_EDITOR
    public void SetData(int index)
    {
        thisSr = GetComponent<SpriteRenderer>();
        spriteIndex = index;
    }

#endif

    #region Value

    [Space(20)]
    [Header("<><><><><> Build Sprite")]
    [SerializeField] private string spriteKey;

    #endregion

    #region Offset

    private void Offset()
    {
        if (DevTool.Get_ComponentTType(gameObject, out SpriteRenderer sr))
        {
            if (spriteKey == "")
                spriteKey = sr.sprite.name.Substring(5, sr.sprite.name.Length - 5);

            StageManager.instance.stageObjectGenerator.Set_CurrentMapSprite(sr, spriteKey);
        }
    }

    #endregion

    #region Framework

    private void Start()
    {
        Offset();
    }

    #endregion
}
