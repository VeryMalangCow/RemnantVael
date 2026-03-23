using UnityEngine;

public class BuildPassageSpriteController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Build Sprite")]
    [SerializeField] private string spriteKey;
    [SerializeField] private bool isBeforeMap = true;

    #endregion

    #region Offset

    private void Offset()
    {
        if (DevTool.Get_ComponentTType(gameObject, out SpriteRenderer sr))
        {
            if (spriteKey == "")
                spriteKey = sr.sprite.name.Substring(5, sr.sprite.name.Length - 5);

            if (isBeforeMap)
                StageManager.instance.Set_BeforeMapSprite(sr, spriteKey);
            else
                StageManager.instance.Set_AfterMapSprite(sr, spriteKey);
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
