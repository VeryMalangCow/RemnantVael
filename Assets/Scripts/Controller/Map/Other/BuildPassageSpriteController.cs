using UnityEngine;

public class BuildPassageSpriteController : MonoBehaviour
{
    #region Value

    [Space(20)]
    [Header("<><><><><> Build Sprite")]
    [SerializeField] private string SpriteKey;
    [SerializeField] private bool IsBeforeMap = true;

    #endregion

    #region Offset

    private void Offset()
    {
        if (DevTool.Get_ComponentTType(gameObject, out SpriteRenderer sr))
        {
            if (SpriteKey == "")
                SpriteKey = sr.sprite.name.Substring(5, sr.sprite.name.Length - 5);

            if (IsBeforeMap)
                StageManager.instance.Set_BeforeMapSprite(sr, SpriteKey);
            else
                StageManager.instance.Set_AfterMapSprite(sr, SpriteKey);
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
